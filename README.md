# distancing-on-sidewalks

Grasshopper plugin
- Target platform: Rhino 7/ Grasshopper

## Introduction

This repository is a prototype solution for evaluating the NYC sidewalks based on the:
- [NYC rhino model provided by the Department of City Planning](https://www1.nyc.gov/site/planning/data-maps/open-data/dwn-nyc-3d-model-download.page)
- [NYC Open Data Sidewalk](https://data.cityofnewyork.us/City-Government/Sidewalk/vfx9-tbb6)

SocialDistancingForSidewalks is a Grasshopper plugin that evaluates the sidewalks and visualises information as a coloured mesh and numerical information. The estimation is based on the sidewalk geometry, relation to specified by user points of interest and people count estimation data.

User specifies the area of interest by drawing boundary and referencing it in Grasshopper. In Grasshopper definition, there are multiple inputs available, which at later stage could be exposed to user interface like HumanUI.

Outputs are visualised in "layers". Those layers are exploded above the default sidewalks for visualisation purposes.

Plugin estimates a number of people that will possibly walk on the sidewalk at the same time during given hour. The values are approximation of the study [Exploring Walking Behavior in the Streets of New York City Using Hourly Pedestrian Count Data](https://www.mdpi.com/2071-1050/12/19/7863/htm#fig_body_display_sustainability-12-07863-f002).

## How to start
Copy files from Grasshopper Plugin folder into your Grasshopper library folder. Then navigate to the Example folder where you will find Rhino and Grasshopper files.
The example file is using Elefront and Urbano plugin. Those can be downloaded from [Food4Rhino](https://www.food4rhino.com/en).

## Functionality<br />

### Data extraction<br />
Firt part of the Grasshopper script (disabled) is reading [NYC Open Data Sidewalk](https://data.cityofnewyork.us/City-Government/Sidewalk/vfx9-tbb6) and converting information into polysurfaces.
Those polysurfaces are baked and used in the second part of workflow.

### Sidewalk evaluation<br />
Three Grasshopper components are available for the sidewalk evaluation. 

**SurfaceCentreline**<br />
This component will attempt to generate centrelines from the surface input. 
The logic of this component is following: https://discourse.mcneel.com/t/extract-centreline-of-polylines/85133/15
![definition2](https://user-images.githubusercontent.com/35227625/142051934-24c873e4-62ac-4bf3-8bbb-9f41e4f494f4.png)


**PeopleCountEstimate**<br />
This component will take into account the sidewalk geometry, average walking speed and time of the day. Based on this information, and the average data from study [Exploring Walking Behavior in the Streets of New York City Using Hourly Pedestrian Count Data](https://www.mdpi.com/2071-1050/12/19/7863/htm#fig_body_display_sustainability-12-07863-f002)., component will estimate how many people is likely to be on this sidewalk at the same time. This number can be later used for the visualisation purposes or for the circle packing algorithm.

![definition](https://user-images.githubusercontent.com/35227625/142049652-2a1da68b-3a20-45c2-b186-2baaf1e72f2a.png)

**Recalculate Weights**<br />
This component allows user to add additional information about location points that can cause more crowds like bustops, subway stations or view points where people tend to stop or walk slower. 

The results can be previewed as a mesh that includes only the given information, or can be embedded into existing values for sidewalk. The second option will visualise a sidewalks with multiple information included: for example width of sidewalk and distance to roadcross (this example can be found in the Grasshopper file)

![definition1](https://user-images.githubusercontent.com/35227625/142051776-6ce5be1a-d591-438f-bcf5-b0a3e12e63df.png)

## My imaginary roadmap
- [] transfering the inputs to HumanUI
- [] back the information with more data like building use
- [] change the 'exploded' visualisation to some sort of dropdown where user can select information they are interested in
- [] extract more information from the [research](https://www.mdpi.com/2071-1050/12/19/7863/htm#fig_body_display_sustainability-12-07863-f002) about varied activity depending on the building use (residential areas will have more activity during morning and evening, business areas will have more activity during lunchtime)

![Capture](https://user-images.githubusercontent.com/35227625/142054984-74206fa3-2f7d-4407-b8e1-e6cfff65e053.PNG)

