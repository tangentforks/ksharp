---
name: investigate-lrs-parsing-problem
description: Investigate test that fails when LRS parsing is enabled
---

When investigating an  reported as an LRS parsing problem
* Keep track of initial parsing mode 
* Enable Legacy Parsing mode
* If already not present, add debugging to identify tokens received by the Legacy Parser and calls to the Evaluator
* Run the test in Legacy Parsing mode and record tokens and evaluator calls
* If the test succeeds the problem is in the LRS parser. Go back to the initial parsing mode identified at the beginning
* Investigate LRS parsing and identify why it is not calling the evaluators that were recorded for the Legacy Parser
* Parsing code is at 
