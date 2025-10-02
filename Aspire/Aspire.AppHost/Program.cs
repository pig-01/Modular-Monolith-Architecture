IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ProjectResource> api = builder.AddProject<Projects.Demo_Demo_Main_WebApi>("mainapi");

builder.AddYarnApp("yarn", "../../../FrontEnd", "dev")
    .WithReference(api).WaitFor(api)
    .WithEndpoint(targetPort: 5173, scheme: "http", name: "frontend");

builder.AddProject<Projects.Demo_Demo_DataHub>("datahubapi");

builder.AddProject<Projects.Demo_Demo_Scheduler>("schedulerapi");

builder.Build().Run();
