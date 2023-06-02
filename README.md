# Black Hole Simulation
Raymarched Black Hole simulation implementing a pseudo photon simulation through HLSL.
<br> Rendered in real-time using Unity
![BlackHoleFinal](https://raw.githubusercontent.com/ScottyRAnderson/Images/master/black-hole-sim_feature_1.jpg)

## Features
* **Volumetric Accretion Disc**
<br> The gravitationally lensed accretion disc is formed from a 3D noise texture sampled in radial coordinate space. This may be controlled through a system of persistent data objects.
<br> Low quality accretion disc option implemented for better performance.
* **Gravitational Blue-shifting**
<br> Simulated the effects of photons gaining energy as they get closer to the singularity. Screen colour is Blue-shifted the closer the observer is.
* **Layered Rendering**
<br> Implemented as a screen-space effect that gets rendered last. Rendering queue therefore allows effects to be stacked such as a bloom filter & colour grading over the black hole.

### Todo
* Implement doppler beaming on the accretion disc.
* Implement gravitational Red-shifting.
* Overlay blue noise to reduce rendering artifacts.

## Development Video
Below is a development video detailing the creation process of this effect,
<br>[Simulations - Black Holes](https://www.youtube.com/watch?v=yhDxBt72PU4)

![GaiaPanorama](https://raw.githubusercontent.com/ScottyRAnderson/Images/master/black-hole-sim_feature_3.jpg)
![BlackHoleDramatic](https://raw.githubusercontent.com/ScottyRAnderson/Images/master/black-hole-sim_feature_2.jpg)

## Known Issues
* **Radial Seams**
<br> Visible seam in the accretion disc where the radial map ends.
* **Multi-Rendering**
<br> Currently, if multiple black holes are rendered at once, they are only affected by those lower in the rendering queue. For a serious multi-rendering approach they would need to be processed in parallel.

## License
This work is licensed under a
[Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License][cc-by-nc-sa].

[![CC BY-NC-SA 4.0][cc-by-nc-sa-image]][cc-by-nc-sa]

[cc-by-nc-sa]: http://creativecommons.org/licenses/by-nc-sa/4.0/
[cc-by-nc-sa-image]: https://licensebuttons.net/l/by-nc-sa/4.0/88x31.png
[cc-by-nc-sa-shield]: https://img.shields.io/badge/License-CC%20BY--NC--SA%204.0-lightgrey.svg