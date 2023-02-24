# Black Hole Simulation
Raymarched Black Hole simulation implementing a pseudo photon simulation through HLSL.
<br> Rendered in real-time using Unity
![BlackHoleFinal](https://raw.githubusercontent.com/ScottyRAnderson/Images/master/BlackHoleFinal.png)

## Features
* **Accretion Disc**
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

![GaiaPanorama](https://raw.githubusercontent.com/ScottyRAnderson/Images/master/GaiaPanorama.png)
![BlackHoleCinematic](https://raw.githubusercontent.com/ScottyRAnderson/Images/master/BlackHoleCinematic.png)

## Known Issues
* **Radial Seams**
<br> Visible seam in the accretion disc where the radial map ends.
* **Multi-Rendering**
<br> Currently, if multiple black holes are rendered at once, they are only affected by those lower in the rendering queue. For a serious multi-rendering approach they would need to be processed in parallel.