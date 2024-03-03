public string WriteCode()
{
    if (Entity.Models.Count == 0)
        return string.Empty;

    var modelNamespace = Entity.Models.Select(m => m.ModelNamespace).FirstOrDefault();
    var readModel = string.Empty;
    var createModel = string.Empty;
    var updateModel = string.Empty;

    foreach (var model in Entity.Models)
    {
        switch (model.ModelType)
        {
            case ModelType.Read:
                readModel = model.ModelClass.ToSafeName();
                break;
            case ModelType.Create:
                createModel = model.ModelClass.ToSafeName();
                break;
            case ModelType.Update:
                updateModel = model.ModelClass.ToSafeName();
                break;
        }
    }

    if (string.IsNullOrEmpty(readModel))
        return string.Empty;

    CodeBuilder.Clear();

    CodeBuilder.AppendLine("using System;");

    if (!string.IsNullOrEmpty(modelNamespace))
        CodeBuilder.AppendLine($"using {modelNamespace};");

    CodeBuilder.AppendLine();

    CodeBuilder.AppendLine($"namespace {TemplateOptions.Namespace};");
    CodeBuilder.AppendLine();

    GenerateClass(readModel, createModel, updateModel);

    CodeBuilder.AppendLine();

    return CodeBuilder.ToString();
}

private void GenerateClass(string readModel, string createModel, string updateModel)
{
    var className = System.IO.Path.GetFileNameWithoutExtension(TemplateOptions.FileName);
    var keyType = TemplateOptions.Parameters["keyType"];

    CodeBuilder.AppendLine("[RegisterScoped]");

    if (string.IsNullOrEmpty(updateModel))
        CodeBuilder.AppendLine($"public class {className} : RepositoryQueryBase<{keyType}, {readModel}, {readModel}>");
    else
        CodeBuilder.AppendLine($"public class {className} : RepositoryUpdateBase<{keyType}, {readModel}, {readModel}, {updateModel}>");

    CodeBuilder.AppendLine("{");

    using (CodeBuilder.Indent())
    {
        GenerateConstructor(className);

        CodeBuilder.AppendLine($"protected override string GetBasePath() => \"/api/{Entity.EntityClass}\";");
    }

    CodeBuilder.AppendLine("}");

}

private void GenerateConstructor(string className)
{
    CodeBuilder.AppendLine($"public {className}(Services.GatewayClient gateway) : base(gateway)");
    CodeBuilder.AppendLine("{");
    CodeBuilder.AppendLine("}");
    CodeBuilder.AppendLine();
}

// run script
WriteCode()
