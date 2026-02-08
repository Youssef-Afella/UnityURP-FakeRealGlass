# UnityURP-FakeRealGlass
Fast glass shader that doesn't require any RayTracing or RayMarching.

## Tech Details
The only expensive part here is rendering the back face texture since we are re-rendering the objects again but it's not really that much (unless all the objects in your game are glass then that will double the cost of your geometry).</br>
The back face ray split is done througth : first refracting the ray on the front face, projecting the point into screen space, then reading the corresponding back normal from the texture to refract again and project again.</br>
You can't project the ray after the first refraction directly without know how much the ray will travel througth the object (which cant done without ray tracing) so as an approximation we consider the thickness of the object at that point as the length that the ray will travel.</br>
The thickness is computed from the substracting the depth of the back faces from the depth of front faces.</br>
The error from this assumption can be quiet high some time but there is no other choose (I believe) also the maximum possible diviation of the refracted ray in glass is 42degree so it will no be a big problem in most cases.</br>
The final assumption we can use here is that most people don't knows how glass is supposed to look like so why caring :).</br>
