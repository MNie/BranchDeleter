module Creator

open Microsoft.TeamFoundation.SourceControl.WebApi
open System

let toDeleteObj repoId (ref: GitRef) =
    let newRef = GitRefUpdate()
    newRef.OldObjectId <- ref.ObjectId
    newRef.NewObjectId <- "0000000000000000000000000000000000000000"
    newRef.Name <- ref.Name
    newRef.RepositoryId <- repoId
    
    newRef

let pRCriteria () =
    let searchCriteria = GitPullRequestSearchCriteria()
    searchCriteria.Status <- Nullable(PullRequestStatus.Active)

    searchCriteria
