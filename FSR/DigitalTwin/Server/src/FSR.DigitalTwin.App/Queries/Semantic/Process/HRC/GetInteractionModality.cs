using System.Text.Json;
using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.Interfaces.Queries.Semantic;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process.HRC.Task;
using FSR.DigitalTwin.Domain.SharedKernel;
using VDS.RDF;
using VDS.RDF.Nodes;


namespace FSR.DigitalTwin.App.Queries.Semantic.Process.HRC;

public class GetInteractionModality : ISparqlQuery<InteractionModality>
{
    private readonly ISparqlServer? _sparqlServer;
    private readonly Resource _simpleTask;

    // TODO Use config paths!
    public string Query => SparqlHelper.LoadQuery("../FSR.DigitalTwin.App/Sparql/GetInteractionModality.sparql", [_simpleTask]);
    public ISparqlResponseParser Parser => new ResponseParser() { SimpleTask = _simpleTask };
    public ISparqlServer SparqlServer { get => _sparqlServer ?? throw new NullReferenceException(); init => _sparqlServer = value; }

    public GetInteractionModality(Resource simpleTask)
    {
        _simpleTask = simpleTask;
    }

    private class ResponseParser : ISparqlResponseParser
    {
        public required Resource SimpleTask { init; get; }

        public IEnumerable<Triple> FromJson(string jsonResponse)
        {
            var json = JsonDocument.Parse(jsonResponse);
            var bindings = json.RootElement.GetProperty("results").GetProperty("bindings");

            var task = (BaseNode)SimpleTask;
            List<Triple> result = [new Triple(task, UriPrefix.RDF | "type", UriPrefix.SOHO | "SimpleTask")];

            if (bindings.GetArrayLength() == 0) 
                throw new InvalidDataException("Missing interaction modality constraints!");
            if (bindings.GetArrayLength() > 2) 
                throw new InvalidDataException("Found multiple interaction modality constraints or more than 2 sub-functions. This is not allowed!");

            var binding1 = bindings.EnumerateArray().First();
            var modality1 = (BaseNode)RdfNodeFactory.CreateFromJson(binding1, "modality");
            var modalityType1 = RdfNodeFactory.CreateFromJson(binding1, "modalityType");
            var function1 = RdfNodeFactory.CreateFromJson(binding1, "function");
            var constraint1 = (BaseNode)RdfNodeFactory.CreateFromJson(binding1, "constraint");
            var constraintType1 = RdfNodeFactory.CreateFromJson(binding1, "constraintType");
            bool isHuman1 = RdfNodeFactory.CreateFromJson(binding1, "isHuman").AsValuedNode().AsBoolean();
            bool isRobot1 = RdfNodeFactory.CreateFromJson(binding1, "isRobot").AsValuedNode().AsBoolean();
            bool isFirst1 = RdfNodeFactory.CreateFromJson(binding1, "isFirst").AsValuedNode().AsBoolean();
            bool isSecond1 = RdfNodeFactory.CreateFromJson(binding1, "isSecond").AsValuedNode().AsBoolean();

            TripleHelper.AddTriple(result, task, UriPrefix.DUL | "isDescribedBy", modality1);
            TripleHelper.AddType(result, task, modalityType1);

            TripleHelper.AddTriple(result, modality1, UriPrefix.DUL | "isDescribedBy", constraint1);
            TripleHelper.AddType(result, constraint1, constraintType1);

            TripleHelper.AddTriple(result, constraint1, UriPrefix.SOHO | "constrains", function1);
            TripleHelper.AddType(result, function1, UriPrefix.SOHO |
                (isHuman1 ? "HumanFunction" : isRobot1 ? "RobotFunction" : "Function"));

            if (isFirst1) TripleHelper.AddTriple(result, constraint1, UriPrefix.SOHO | "isFirstTask", function1);
            else if (isSecond1) TripleHelper.AddTriple(result, constraint1, UriPrefix.SOHO | "isSecondTask", function1);

            if (bindings.GetArrayLength() == 2)
            {
                var binding2 = bindings.EnumerateArray().Last();
                var modality2 = (BaseNode)RdfNodeFactory.CreateFromJson(binding2, "modality");
                var constraint2 = (BaseNode)RdfNodeFactory.CreateFromJson(binding2, "constraint");
                if (modality1 != modality2 || constraint1 != constraint2)
                {
                    throw new InvalidDataException("Found multiple interaction modality constraints. This is not allowed!");
                }
                var function2 = RdfNodeFactory.CreateFromJson(binding2, "function");
                bool isHuman2 = RdfNodeFactory.CreateFromJson(binding2, "isHuman").AsValuedNode().AsBoolean();
                bool isRobot2 = RdfNodeFactory.CreateFromJson(binding2, "isRobot").AsValuedNode().AsBoolean();
                bool isFirst2 = RdfNodeFactory.CreateFromJson(binding2, "isFirst").AsValuedNode().AsBoolean();
                bool isSecond2 = RdfNodeFactory.CreateFromJson(binding2, "isSecond").AsValuedNode().AsBoolean();

                TripleHelper.AddTriple(result, constraint2, UriPrefix.SOHO | "constrains", function2);
                TripleHelper.AddType(result, function2, UriPrefix.SOHO |
                    (isHuman2 ? "HumanFunction" : isRobot2 ? "RobotFunction" : "Function"));

                if (isFirst2) TripleHelper.AddTriple(result, constraint2, UriPrefix.SOHO | "isFirstTask", function2);
                else if (isSecond2) TripleHelper.AddTriple(result, constraint2, UriPrefix.SOHO | "isSecondTask", function2);
            }

            return result;
        }
    }

    public Result<InteractionModality> Run()
    {
        var response = SparqlServer.Query(this);
        if (response.IsFailure)
        {
            return Result.Failure<InteractionModality>(response.Error);
        }
        try
        {
            var modality = MapToDomain(response.Value);
            return Result.Success(modality);
        }
        catch (InvalidDataException)
        {
            return Result.Failure<InteractionModality>(Error.QueryFailed);
        }
        
    }

    public async Task<Result<InteractionModality>> RunAsync(CancellationToken cancellationToken = default)
    {
        var response = await SparqlServer.QueryAsync(this, cancellationToken);
        if (response.IsFailure)
        {
            return Result.Failure<InteractionModality>(response.Error);
        }
        try
        {
            var modality = MapToDomain(response.Value);
            return Result.Success(modality);
        }
        catch (InvalidDataException)
        {
            return Result.Failure<InteractionModality>(Error.QueryFailed);
        }
    }
    
    private static InteractionModality MapToDomain(IEnumerable<Triple> triples)
    {
        var taskNode = triples
            .FirstOrDefault(t => t.Predicate.Equals(UriPrefix.RDF | "type") &&
                                t.Object.Equals(UriPrefix.SOHO | "SimpleTask"))
            ?.Subject
            ?? throw new InvalidDataException("No SimpleTask found in triples.");
        var modalityNode = triples
            .FirstOrDefault(t => t.Subject.Equals(taskNode) &&
                                t.Predicate.Equals(UriPrefix.DUL | "isDescribedBy"))
            ?.Object
            ?? throw new InvalidDataException("No InteractionModality found for SimpleTask.");
        var modalityTypeNode = triples
            .FirstOrDefault(t => t.Subject.Equals(taskNode) &&
                                t.Predicate.Equals(UriPrefix.RDF | "type"))
            ?.Object
            ?? throw new InvalidDataException("No ModalityType found.");
        var constraintNodes = triples
            .Where(t => t.Subject.Equals(modalityNode) &&
                        t.Predicate.Equals(UriPrefix.DUL | "isDescribedBy"))
            .Select(t => t.Object)
            .ToList();

        if (constraintNodes.Count != 1)
        {
            throw new InvalidDataException("Expected exactly one constraint for InteractionModality.");
        }

        var constraintNode = constraintNodes.Single();

        var constrainedFunctions = triples
            .Where(t => t.Subject.Equals(constraintNode) &&
                        t.Predicate.Equals(UriPrefix.SOHO | "constrains"))
            .Select(t => t.Object)
            .ToList();

        Resource? firstFunction = null;
        Resource? secondFunction = null;
        Resource? humanFunction = null;
        Resource? robotFunction = null;

        foreach (var f in constrainedFunctions)
        {
            var typeTriple = triples.FirstOrDefault(t => t.Subject.Equals(f) &&
                                                        t.Predicate.Equals(UriPrefix.RDF | "type"));

            if (typeTriple != null)
            {
                if (typeTriple.Object.Equals(UriPrefix.SOHO | "HumanFunction"))
                    humanFunction = f.AsResource();
                else if (typeTriple.Object.Equals(UriPrefix.SOHO | "RobotFunction"))
                    robotFunction = f.AsResource();
            }

            if (triples.Any(t => t.Subject.Equals(constraintNode) &&
                                t.Predicate.Equals(UriPrefix.SOHO | "isFirstTask") &&
                                t.Object.Equals(f)))
            {
                firstFunction = f.AsResource();
            }

            if (triples.Any(t => t.Subject.Equals(constraintNode) &&
                                t.Predicate.Equals(UriPrefix.SOHO | "isSecondTask") &&
                                t.Object.Equals(f)))
            {
                secondFunction = f.AsResource();
            }
        }

        return new InteractionModality
        {
            Resource = taskNode.AsResource(),
            Type = modalityTypeNode.AsResource(),
            Function1 = firstFunction ?? throw new NullReferenceException("should not happen"),
            Agent1 = firstFunction == humanFunction ? HRCTask.EAgent.Human :
                firstFunction == robotFunction ? HRCTask.EAgent.Robot : HRCTask.EAgent.Any,
            Function2 = secondFunction,
            Agent2 = secondFunction == null ? HRCTask.EAgent.Any : secondFunction == robotFunction ?
                HRCTask.EAgent.Robot : secondFunction == humanFunction ?
                HRCTask.EAgent.Human : HRCTask.EAgent.Any
        };
    }
}