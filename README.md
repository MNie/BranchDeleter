# BranchDeleter

F# utility Azure Function to periodically delete stale branches from AzureDevops.
Requirements that branches should met to be deleted:
- a branch should be older than 1 month;
- a branch should not have any open pull requests;
- the branch name is not equal to test, nor beta, nor release

More about how it was done you could read [here](https://www.mnie.me/2019-09-23-branchDeleter/)

Feel free to fork and adjust to your needs!
