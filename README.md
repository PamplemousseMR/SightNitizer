# SightProperties

This project allow to check bundles and libraries in Sight.
* Check that appxml and fwlauncher are in the Properties.cmake.
* Check that bundle in xml file are properly started (requirement in xml files).
* Check that bundles used in xml files are in the Properties.cmake (REQUIREMENT).
* Check that library used in languages files are in the Properties.cmake (DEPENDENCIES).
* Check that bundles and libraries in the Properties.cmake are used.
* Check that all services are used.
* Check that all objects are used.
* Check that all channel are used.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Compilation

Open `SightProperties.sln` and compile by using visual studio.

### Use

Use the utility with a bundle/library/app : `SightProperties.exe <path>`.

```
- path : path to the bundle/library/app to check.
```

Use the utility with a repository : `SightProperties.exe <path>`.

```
- path : path to the repository.
```

## Authors

* **MANCIAUX Romain** - *Initial work* - [rmanciaux](https://git.ircad.fr/rmanciaux).