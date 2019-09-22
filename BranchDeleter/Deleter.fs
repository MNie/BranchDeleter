module Deleter

open Microsoft.Extensions.Logging
open System
open Microsoft.TeamFoundation.SourceControl.WebApi
open Microsoft.VisualStudio.Services.Client
open Microsoft.VisualStudio.Services.Common
open Microsoft.VisualStudio.Services.WebApi
open System.Threading

let private connection(config: Configuration.Config) = 
    let basicCred = new VssBasicCredential("", config.accessToken)
    let creds = new VssClientCredentials(basicCred)
    
    new VssConnection(new Uri(config.repository), creds)

let private deleteBranchesFrom (config: Configuration.Config) (client: GitHttpClient) (log: ILogger) (repo: GitRepository) =
    let createRefToDelete = Creator.toDeleteObj repo.Id
    log.LogInformation (sprintf "repository: %A" repo.Name)
    async {
        let! branchesWithPR = client.GetPullRequestsByProjectAsync(config.project, Creator.pRCriteria ()) |> Async.AwaitTask
        let! branches = client.GetRefsAsync(config.project, repo.Id, "", Nullable<bool>(), Nullable<bool>(), Nullable<bool>(), Nullable<bool>(), Nullable<bool>(), "", null, CancellationToken.None) |> Async.AwaitTask

        let branchesToDelete = 
            Filter.notDeletable branches branchesWithPR
            |> Seq.map createRefToDelete
        
        match branchesToDelete with
        | toDelete when toDelete |> Seq.length > 0 ->
            log.LogInformation (sprintf "%A" (toDelete |> Seq.map (fun x -> x.Name) |> Seq.toArray))

            let! result = client.UpdateRefsAsync(toDelete, repo.Id, config.project, null, CancellationToken.None) |> Async.AwaitTask
                    
            result 
            |> Seq.filter (fun x -> x.UpdateStatus.IsSuccessful() = false) 
            |> Seq.iter (fun x -> log.LogWarning (sprintf "Something bad happened during delete branch with ref: %s \r\nmessage: %s" x.Name x.CustomMessage))
        | _ -> ()
            
        do log.LogInformation ("done")
    } |> Async.StartAsTask
    
    
let run (config: Configuration.Config) (log) =
    let conn = connection(config)
    let client = conn.GetClient<GitHttpClient>()
    let deleteFunc = deleteBranchesFrom config client log
    async {
        let! repositories = client.GetRepositoriesAsync(config.project, Nullable(false)) |> Async.AwaitTask

        do! 
            repositories
            |> Seq.map (fun repo -> async { do! deleteFunc (repo) |> Async.AwaitTask })
            |> Async.Parallel
            |> Async.Ignore
    } |> Async.StartAsTask


