# distancing-on-sidewalks

- Author: Justyna Szychowska
- Target platform: Rhino 7/ Grasshopper

## Introduction

This repository is a prototype solution for evaluating the NYC sidewalks based on the:
- [NYC rhino model provided by the Department of City Planning](https://www1.nyc.gov/site/planning/data-maps/open-data/dwn-nyc-3d-model-download.page)
- [NYC Open Data Sidewalk](https://data.cityofnewyork.us/City-Government/Sidewalk/vfx9-tbb6)

## How to start
Copy files from Grasshopper Plugin folder into your Grasshopper library folder. Then navigate to the Example folder where you will find Rhino and Grasshopper files.

## Functionality
### Data extraction
Firt part of the Grasshopper script (disabled) is reading [NYC Open Data Sidewalk](https://data.cityofnewyork.us/City-Government/Sidewalk/vfx9-tbb6) and converting information into polysurfaces.
Those polysurfaces are baked and later used in the second part of workflow.

### Sidewalk evaluation
Three Grasshopper components are available for the sidewalk evaliation. 

SurfaceCentreline
This component will attempt to generate centrelines from the surface input. 

