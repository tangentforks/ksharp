# Intake: update the project plan and documentation in preparation for further work
[ ] First start running in the background a full comparison using the instructions in T:\\_src\\github.com\\ERufian\\ksharp\\K3CSharp.Comparison\\README.md. 
    - This test may be slow and you want to leave it running while you are doing other work, that is why we want to start it at the beginning. 
[ ] Check periodically if the full comparison that was running in the background has finished running
[ ] Update the current project plan by comparing current functionality to the functionality described in T:\\_src\\github.com\\ERufian\\vibe-docs\\ksharp\\cleaned\\*.html 
    - Take into account that the UI and related attributes are excluded from the scope 
    - Take into account that we want to implement a Foreign Function Interface for interoperating with .NET. 
[ ] Identify if there is new functionality that is not yet described in T:\\_src\\github.com\\ERufian\\ksharp\\README.md 
[ ] Identify if there is new functionality that is not yet described in T:\\_src\\github.com\\ERufian\\vibe-docs\\ksharp\\K3CSharp_implementation_details.md 
[ ] Describe the identified new functionality from the user's perspective in T:\\_src\\github.com\\ERufian\\ksharp\\README.md. 
[ ] Describe the identified new functionality from the developer perspective in T:\\_src\\github.com\\ERufian\\vibe-docs\\ksharp\\K3CSharp_implementation_details.md. 
[ ] Identify if there is obsolete functionality in T:\\_src\\github.com\\ERufian\\ksharp\\README.md that should be removed
    - Make sure that you preserve general sections such as Installation instructions and Authorship.
[ ] Eliminate from T:\\_src\\github.com\\ERufian\\vibe-docs\\ksharp\\K3CSharp_implementation_details.md any references to test and comparison statistics, because those become obsolete quickly and they aren't really implementation details. 
[ ] Identify if there are sections in T:\\_src\\github.com\\ERufian\\vibe-docs\\ksharp\\K3CSharp_implementation_details.md that are contradicted by later developments
[ ] Tag  as obsolete the sections in T:\\_src\\github.com\\ERufian\\vibe-docs\\ksharp\\K3CSharp_implementation_details.md that are contradicted by later developments
     - Include a link to the section that indicates the later change that makes it obsolete. 
[ ] Calculate project completion statistics based on the latest project plan 
[ ] Use updated project completion statistics to update the number in T:\\_src\\github.com\\ERufian\\ksharp\\README.md
    - Ensure that the percentage of completion is mentioned only once
    - If completion statistics are mentioned multiple times, then eliminate the redundant ones. 
[ ] Run a full test pass using the test runner in T:\\_src\\github.com\\ERufian\\ksharp\\K3CSharp.Tests 
[ ] Use the result table produced by the test runner to update the numbers for test statistics in T:\\_src\\github.com\\ERufian\\ksharp\\README.md
    - Ensure that the numbers for passing and failng tests and percentage of success is mentioned only once
    - If the numbers for passing and failng tests and/or percentage of success are mentioned multiple times then eliminate the redundant ones. 
    - Make sure you run the test runner from its folder, so that it can correctly find the latest test scripts with the correct path.
[ ] If the full comparison that was started earlier is still running, wait for it to complete
[ ] After the full comparison that was started earlier has finished running, use its summary results to update the numbers for compatibility statistics in T:\\_src\\github.com\\ERufian\\ksharp\\README.md
    - Ensuring that the numbers for matching, differed skipped, errors and percentage of compatibility is mentioned only once
    - If the numbers for matching, differed skipped, errors and percentage of compatibility are mentioned multiple times, then eliminate the redundant ones.
