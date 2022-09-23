# Contributing

Please first discuss the change you wish to make via an issue before making a change. 
 
## Testing 

1. Set up powerapps binding
2. Application user musy be added with the system admin rights
3. Tests by default is on US datetime format, therefore change it based on your personal location. For example, if you are based in India, set the datetime format to the local time. 
- This can be done by going to the Power Apps settings > 
- Select Pernalization Settings > 
- From the Set Personal Options page, click on the Formats menu > 
- Change the Current Format from the dropdown by ther selecting the country of your location > 
- Click on the Customize button > 
- Click OK button

NB: Running all tests are not required as the CI build will do these. Test verify any tests you have impacted.

## Pull request process

1. Ensure that there are automated tests that cover any changes 
2. Update the README.md with details of any significant changes to functionality
3. Ensure that your commit messages increment the version using [GitVersion syntax](https://gitversion.readthedocs.io/en/latest/input/docs/more-info/version-increments/). If no message is found then the patch version will be incremented by default.
4. You may merge the pull request once it meets all of the required checks. If you do not have permision, a reviewer will do it for you