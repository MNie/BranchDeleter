module Runner

open Microsoft.Azure.WebJobs
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration
open Configuration

[<FunctionName("BranchDeleter")>]
let deleteStaleBranches ([<TimerTriggerAttribute("0 0 6 * * *")>]myTimer: TimerInfo, log: ILogger, context: Microsoft.Azure.WebJobs.ExecutionContext) =
    let builder = new ConfigurationBuilder()
    let configuration = builder.SetBasePath(context.FunctionAppDirectory).AddJsonFile("settings.json", true, true).AddEnvironmentVariables().Build()
    let config = {
        repository = configuration.["AzureDevops:Repository"]
        project = configuration.["AzureDevops:Project"]
        accessToken = configuration.["AzureDevops:AccessToken"]
    }
    async {
        async { do! (Deleter.run config log) |> Async.AwaitTask } |> Async.RunSynchronously
    } |> Async.StartAsTask |> ignore

