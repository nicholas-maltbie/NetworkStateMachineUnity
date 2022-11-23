# Template Unity Package Introduction

Introduction to Nick Maltbie's Network state Machine Unity project..

Extension of the [StateMachineUnity](https://nickmaltbie.com/StateMachineUnity/docs/)
project to work with [Unity's netcode package](https://docs-multiplayer.unity3d.com/netcode/current/about)

to allow for managing a state machine over a network.

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
