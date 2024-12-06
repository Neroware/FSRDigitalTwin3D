#!/usr/bin/env python

# !!!!!!!!!!! THIS SCRIPT WAS INSTALLED AND WILL BE OVERWRITTEN WHEN BUILD RE-INITS !!!!!!!!!!

from __future__ import print_function

from threading import Thread

import rclpy
from rclpy.node import Node
from rclpy.callback_groups import ReentrantCallbackGroup

import sys
import copy
import math
from pymoveit2 import MoveIt2, MoveIt2State
# import moveit_commander

import moveit_msgs.msg
from moveit_msgs.msg import Constraints, JointConstraint, PositionConstraint, OrientationConstraint, BoundingVolume
from sensor_msgs.msg import JointState
from moveit_msgs.msg import RobotState
import geometry_msgs.msg
from geometry_msgs.msg import Quaternion, Pose
from std_msgs.msg import String
# from moveit_commander.conversions import pose_to_list

# ---

# from moveit_msgs.srv import (
#     ApplyPlanningScene,
#     GetCartesianPath,
#     GetMotionPlan,
#     GetPlanningScene,
#     GetPositionFK,
#     GetPositionIK,
# )
# from rclpy.qos import (
#     QoSDurabilityPolicy,
#     QoSHistoryPolicy,
#     QoSProfile,
#     QoSReliabilityPolicy,
# )

# ---

from ur5e_moveit.srv import MoverService

joint_names = ['joint_1', 'joint_2', 'joint_3', 'joint_4', 'joint_5', 'joint_6']

if True:
    def planCompat(plan):
        return plan
else:
    raise NotImplementedError()

class UR5e_MoveIt_Server(Node):
    def __init__(self):
        super().__init__('ur5e_moveit_server')
        self._mover_srv = self.create_service(MoverService, 'ur5e_moveit', self._plan_pick_and_place)
    
    """
    Creates a pick and place plan using the four states below.
    
    1. Pre Grasp - position gripper directly above target object
    2. Grasp - lower gripper so that fingers are on either side of object
    3. Pick Up - raise gripper back to the pre grasp position
    4. Place - move gripper to desired placement position

    Gripper behaviour is handled outside of this trajectory planning.
        - Gripper close occurs after 'grasp' position has been achieved
        - Gripper open occurs after 'place' position has been achieved

    https://github.com/ros-planning/moveit/blob/master/moveit_commander/src/moveit_commander/move_group.py
    """
    def _plan_pick_and_place(self, req, res):
        self.get_logger().info("Recieved request to plan trajectory for UR5e cobot...")

        callback_group = ReentrantCallbackGroup()

        group_name = "arm"
        end_effector_name = "ee_link"
        base_link_name = "base"
        planner_id = "RRTConnectkConfigDefault"

        move_group = MoveIt2(
            node=self,
            joint_names=joint_names,
            base_link_name=base_link_name,
            end_effector_name=end_effector_name,
            group_name=group_name,
            callback_group=callback_group,
        )
        move_group.planner_id = ( planner_id )

        # Spin the node in background thread(s) and wait a bit for initialization
        executor = rclpy.executors.MultiThreadedExecutor(2)
        executor.add_node(self)
        executor_thread = Thread(target=executor.spin, daemon=True, args=())
        executor_thread.start()
        self.create_rate(1.0).sleep()

        # ---

        # plan_kinematic_path_service = self.create_client(
        #     srv_type=GetMotionPlan,
        #     srv_name="plan_kinematic_path",
        #     qos_profile=QoSProfile(
        #         durability=QoSDurabilityPolicy.VOLATILE,
        #         reliability=QoSReliabilityPolicy.RELIABLE,
        #         history=QoSHistoryPolicy.KEEP_LAST,
        #         depth=1,
        #     ),
        #     callback_group=callback_group,
        # )
        # while not plan_kinematic_path_service.service_is_ready():
        #     executor = rclpy.executors.MultiThreadedExecutor(2)
        #     executor.add_node(self)
        #     executor_thread = Thread(target=executor.spin, daemon=True, args=())
        #     executor_thread.start()
        #     self.create_rate(1.0).sleep()
        #     self.get_logger().info("Waiting for service...")

        # ---

        current_robot_joint_configuration = req.joints_input.joints

        # Pre grasp - position gripper directly above target object
        pre_grasp_pose = self._plan_trajectory(move_group, req.pick_pose, current_robot_joint_configuration)
    

        # Pre grasp - position gripper directly above target object
        # pre_grasp_pose = self._plan_trajectory(None, req.pick_pose, current_robot_joint_configuration)

        # # If the trajectory has no points, planning has failed and we return an empty response
        # if not pre_grasp_pose.joint_trajectory.points:
        #     return res

        return res
    
    """
    Given the start angles of the robot, plan a trajectory that ends at the destination pose.
    """
    def _plan_trajectory(self, move_group, destination_pose, start_joint_angles):
        start_joint_angles = start_joint_angles.tolist()

        current_joint_state = JointState()
        current_joint_state.name = joint_names
        current_joint_state.position = start_joint_angles

        moveit_robot_state = RobotState()
        moveit_robot_state.joint_state = current_joint_state

        move_group.max_velocity = 0.5
        move_group.max_acceleration = 0.5

        self.get_logger().info(f"Moving to start state {{joint_positions: {list(start_joint_angles)}}}")
        plan = move_group.plan(start_joint_state=current_joint_state, pose=destination_pose)
        self.get_logger().info(f">>> " + str(plan))
        # move_group.move_to_configuration(start_joint_angles)
        # move_group.wait_until_executed()

        # return None
        # current_joint_state = JointState()
        # current_joint_state.name = joint_names
        # current_joint_state.position = start_joint_angles

        # moveit_robot_state = RobotState()
        # moveit_robot_state.joint_state = current_joint_state
        # # move_group.set_start_state(moveit_robot_state)

        # move_group.set_pose_target(destination_pose)
        plan = None # move_group.plan()
        return plan

        if not plan:
            exception_str = """
                Trajectory could not be planned for a destination of {} with starting joint angles {}.
                Please make sure target and destination are reachable by the robot.
            """.format(destination_pose, destination_pose)
            raise Exception(exception_str)

        return planCompat(plan)

def main(args=None):
    rclpy.init(args=args)

    moveit_server = UR5e_MoveIt_Server()

    try:
        rclpy.spin(moveit_server)
    except KeyboardInterrupt:
        pass
    finally:
        moveit_server.destroy_node()
        rclpy.shutdown()


if __name__ == '__main__':
    main()