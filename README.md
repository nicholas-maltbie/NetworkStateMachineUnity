# Network State Machine Unity

Nick Maltbie's Network State Machine Unity
as a networking extension for the
[https://github.com/nicholas-maltbie/StateMachineUnity](https://github.com/nicholas-maltbie/StateMachineUnity)
project.

If you want to host and share code between unity projects easily,
use this project to configure unity builds.

Network State Machine Unity is an open source project hosted at
[https://github.com/nicholas-maltbie/NetworkStateMachineUnity](https://github.com/nicholas-maltbie/NetworkStateMachineUnity)

This is an open source project licensed under a [MIT License](LICENSE.txt).

## Installation

Install the latest version of the project by importing a project via git
at this URL:
`https://github.com/nicholas-maltbie/NetworkStateMachineUnity.git#release/latest`

If you want to reference a specific tag of the project such as version `v1.0.0`,
add a `release/#v1.0.0` to the end of the git URL to download the package
from th auto-generated branch for that release. An example of importing `v1.0.0`
would look like this:

```text
https://github.com/nicholas-maltbie/NetworkStateMachineUnity.git#release/v0.1.0
```

To use the latest release, simply reference:

```text
https://github.com/nicholas-maltbie/NetworkStateMachineUnity.git#release/latest
```

For a full list of all tags, check the
[NetworkStateMachineUnity Tags](https://github.com/nicholas-maltbie/NetworkStateMachineUnity/tags)
list on github. I will usually associated a tag with each release of the project.

You can also import the project via a tarball if you download the source
code and extract it on your local machine. Make sure to import
via the package manifest defined at `Packages\com.nickmaltbie.NetworkStateMachineUnity\package.json`
within the project.

For more details about installing a project via git, see unity's documentation
on [Installing form a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html#:~:text=%20Select%20Add%20package%20from%20git%20URL%20from,repository%20directly%20rather%20than%20from%20a%20package%20registry.).

### Scoped Registry Install

If you wish to install the project via a
[Scoped Registry](https://docs.unity3d.com/Manual/upm-scoped.html)
and npm, you can add a scoped registry to your project from all of the
`com.nickmaltbie` packages like this:

```json
"scopedRegistries": [
  {
    "name": "nickmaltbie",
    "url": "https://registry.npmjs.org",
    "scopes": [
      "com.nickmaltbie"
    ]
  }
]
```

Then, if you want to reference a version of the project, you simply
need to include the dependency with a version string and the unity package
manager will be able to download it from the registry at
`https://registry.npmjs.org`

```json
"dependencies": {
    "com.nickmaltbie.networkstatemachineunity": "1.0.0",
    "com.nickmaltbie.testutilsunity" : "0.0.2",
    "com.nickmaltbie.statemachineunity" : "1.1.2",
    "com.unity.inputsystem": "1.0.0",
    "com.unity.netcode.gameobjects": "1.1.0"
}
```

## Development

This project is developed using Unity Release [2021.3.11f1](https://unity3d.com/unity/whats-new/2021.3.11).
Install this version of Unity from Unity Hub using this link:
[unityhub://2021.3.11f1/0a5ca18544bf](unityhub://2021.3.11f1/0a5ca18544bf).

## Documentation

Documentation on the project and scripting API is found at
[https://nickmaltbie.com/NetworkStateMachineUnity/docs/](https://nickmaltbie.com/NetworkStateMachineUnity/docs/)
for the latest version of the codebase.

## Demo

You can see a demo of the project running here:
[https://nickmaltbie.com/NetworkStateMachineUnity/](https://nickmaltbie.com/NetworkStateMachineUnity/).
The project hosted on the website is up to date with the most recent
version on the `main` branch of this github repo
and is automatically deployed with each update to the codebase.

## Samples

The samples in the project include:

* ExampleSample - Example sample for Network State Machine Unity.
