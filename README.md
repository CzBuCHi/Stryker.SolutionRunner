Simple wrapper around [stryker-net](https://github.com/stryker-mutator/stryker-net) that will try mutate all projects in solution.

Usage:
Stryker.SolutionRunner Solution [--force] [stryker args]

- Solution: full path to solution
- --force: regenerate all reports
- [stryker args]: extra argumnets for stryker
   note: program implicitly uses `-s SOLUTION -p PROJECT -tp TEST_PROJECT -r json -O StrykerOutput`


Report is generated only if:
a) --force flag is used
b) any file in test/tested project (excluding files in bin & obj) is newer than report file.

Report file for given test project is stored in TEST_PROJECT_NAME.json

Final merged report is stored as SOLUTION_NAME.html