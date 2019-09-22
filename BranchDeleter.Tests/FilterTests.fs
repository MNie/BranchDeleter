module FilterTests 

    open System
    open Microsoft.TeamFoundation.SourceControl.WebApi
    open Xunit
    open Swensen.Unquote

    [<Fact>]
    let ``When our branch has name papaj and has no opened pull requests and was last updated 2 days ago`` () =
        let gitRef = new GitRef()
        gitRef.Name <- "papaj"
        let status = new GitStatus()
        let status2 = new GitStatus()
        status.CreationDate <- new DateTime(2010, 1, 1)
        status.UpdatedDate <- DateTime.UtcNow.AddDays(-20.)
        status2.UpdatedDate <- DateTime.UtcNow.AddDays(-40.)
        gitRef.Statuses <- [| status; status2 |]
        let branches = [
            gitRef
        ]

        let pullRequest = new GitPullRequest()
        pullRequest.SourceRefName <- "papaj2"
        let pullrequests = [
            pullRequest
        ]
    
        let result = Filter.notDeletable branches pullrequests
        (result |> Seq.toArray |> Seq.length) =! 0

    [<Fact>]
    let ``When our branch has name papaj and has no opened pull requests and was last updated 2 weeks ago`` () =
        let gitRef = new GitRef()
        gitRef.Name <- "papaj"
        let status = new GitStatus()
        let status2 = new GitStatus()
        status.CreationDate <- new DateTime(2010, 1, 1)
        status.UpdatedDate <- DateTime.UtcNow.AddDays(-35.)
        status2.UpdatedDate <- DateTime.UtcNow.AddDays(-45.)
        gitRef.Statuses <- [| status; status2 |]
        let branches = [
            gitRef
        ]
    
        let pullRequest = new GitPullRequest()
        pullRequest.SourceRefName <- "papaj2"
        let pullrequests = [
            pullRequest
        ]
    
        let result = Filter.notDeletable branches pullrequests
        (result |> Seq.head) =! gitRef
        (result |> Seq.toArray |> Seq.length) =! 1

    [<Fact>]
    let ``When our branch has name papaj and has opened pull requests and was last updated 2 weeks ago`` () =
        let gitRef = new GitRef()
        gitRef.Name <- "papaj"
        let status = new GitStatus()
        let status2 = new GitStatus()
        status.CreationDate <- new DateTime(2010, 1, 1)
        status.UpdatedDate <- DateTime.UtcNow.AddDays(-45.)
        status2.UpdatedDate <- DateTime.UtcNow.AddDays(-35.)
        gitRef.Statuses <- [| status; status2 |]
        let branches = [
            gitRef
        ]
    
        let pullRequest = new GitPullRequest()
        pullRequest.SourceRefName <- "papaj"
        let pullrequests = [
            pullRequest
        ]
    
        let result = Filter.notDeletable branches pullrequests
        (result |> Seq.toArray |> Seq.length) =! 0

    [<Fact>]
    let ``When our branch has name release and has not opened pull requests and was last updated 2 weeks ago`` () =
        let gitRef = new GitRef()
        gitRef.Name <- "release"
        let status = new GitStatus()
        let status2 = new GitStatus()
        status.CreationDate <- new DateTime(2010, 1, 1)
        status.UpdatedDate <- DateTime.UtcNow.AddDays(-35.)
        status2.UpdatedDate <- DateTime.UtcNow.AddDays(-40.)
        gitRef.Statuses <- [| status; status2 |]
        let branches = [
            gitRef
        ]
    
        let pullRequest = new GitPullRequest()
        pullRequest.SourceRefName <- "papaj2"
        let pullrequests = [
            pullRequest
        ]
    
        let result = Filter.notDeletable branches pullrequests
        (result |> Seq.toArray |> Seq.length) =! 0