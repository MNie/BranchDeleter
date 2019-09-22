module CreatorTests 

    open System
    open Microsoft.TeamFoundation.SourceControl.WebApi
    open Xunit
    open Swensen.Unquote

    [<Fact>]
    let ``When creating object to delete it should has oldobjectid from ref, name from rep, repoid set and the newobjectidshould be 0`` () =
        let gitRef = new GitRef ()
        gitRef.Name <- "papaj"
        gitRef.ObjectId <- "objectIdFromGitRef"

        let result = Creator.toDeleteObj (Guid.Parse "7c12e877-4198-41db-ae93-d338d44e4bc8") gitRef

        result.OldObjectId =! "objectIdFromGitRef"
        result.NewObjectId =! "0000000000000000000000000000000000000000"
        result.Name =! "papaj"
        result.RepositoryId =! (Guid.Parse "7c12e877-4198-41db-ae93-d338d44e4bc8")

    [<Fact>]
    let ``When getting pull request criteria, the only thing that matter is that pull request should be still open``() =
        let result = Creator.pRCriteria ()

        result.Status.Value =! PullRequestStatus.Active

