# EnvironmentConfig

Out of the box class that contains the current environment name the app is running in

# Usage

EnvironmentConfig.Instance.Name contains the current environment set based on the default way supported by Microsoft: throug ASPNET_ENVIRONMENTNAME variable

Inherit EnvironmentConfig to add additional environment configurations by:
- creating a file at /Configs/environmentConfig.json
- add any property you additionally need within the json file
- add for instance transformation in production by creating and adding variables to
  - /Configs/environmentConfig.production.json