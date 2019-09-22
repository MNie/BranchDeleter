module Filter

    open Microsoft.TeamFoundation.SourceControl.WebApi
    open System
    let private baseBranchesToExclude = [ 
            "master"; "test"; "beta"; "release"; "int" 
            "refs/heads/master"; "refs/heads/test"; "refs/heads/beta"; "refs/heads/release"; "refs/heads/int" 
        ]

    let notDeletable (branches: GitRef seq) (branchesWithPR: GitPullRequest seq) =
        let monthAgo = DateTime.UtcNow.AddMonths (-1)
    
        branches
        |> Seq.filter (fun x ->
            if (baseBranchesToExclude |> Seq.filter (fun y -> y = x.Name) |> Seq.length > 0) then false
            elif (branchesWithPR |> Seq.exists (fun y -> y.SourceRefName = x.Name)) then false
            elif (x.Statuses |> Seq.map (fun y -> y.UpdatedDate) |> Seq.sortDescending |> Seq.head |> fun z -> z > monthAgo) then false
            else true
        )