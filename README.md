# UnityURP-FakeRealGlass
Fast glass shader that doesn't require any RayTracing or RayMarching.
It's not 100% accurate but it's probably the best possible thing we can do with what we have.

## How to Use
First, start by adding the "BackFaceNormalsTexture" feature then choosing the layermask for glass objects.
<img width="638" height="133" alt="image" src="https://github.com/user-attachments/assets/dda0c71b-0e2e-4dbe-934a-7728c9971ff2" />

Next make a material from shader "Glass Double Face" or "Glass Single Face" (single face is better for spherical objects)
<img width="646" height="514" alt="image" src="https://github.com/user-attachments/assets/f229567d-33d6-47ca-87f1-86fa5c5a1ac7" />

Some parameters there are kinda strange so I explained them in the "Performance/Tech Details" section.

## Screenshots
<img width="1920" height="1080" alt="Image Sequence_002_0000" src="https://github.com/user-attachments/assets/fff4662b-9be9-4185-8cc3-3030b12ef177" />
<img width="1920" height="1080" alt="Image Sequence_001_0000" src="https://github.com/user-attachments/assets/475f82cf-8482-45b2-9b3e-ebe934e760df" />
<img width="1920" height="1080" alt="Image Sequence_003_0000" src="https://github.com/user-attachments/assets/5c9ff322-e7b6-4455-bbbc-b91a5f84c0de" />
<img width="1920" height="1080" alt="Image Sequence_005_0000" src="https://github.com/user-attachments/assets/eba7a25d-64ff-4b06-aa91-9c00e995c1dc" />

## Performance/Tech Details
The number of texture samples we do here is very low so it's probably fast enougth to be used in mobile or VR (didn't tested).</br>
The only expensive part here is rendering the back face texture since we are re-rendering the objects again but it's not really that much (unless all the objects in your game are glass then that will double the cost of your geometry).

The back face ray split is done througth : first refracting the ray on the front face, projecting the point into screen space, then reading the corresponding back normal from the texture to refract again and project again.</br>
You can't project the ray after the first refraction directly without knowing how much the ray will travel througth the object (which cant done without ray tracing) so as an approximation we consider the thickness of the object at that point as the length that the ray will travel.</br>
The thickness is computed from the substracting the depth of the back faces from the depth of front faces.</br>
The error from this assumption can be quiet high some time but there is no other choice (I believe) also the maximum possible diviation of the refracted ray in glass is 42degree so it will no be a big problem in most cases.
One other parameter in the material is the "UseConstantThickness" which simply replaces the depth based thickness with a constant value, it can look better in some objects.

For the second refraction (which is assumed to be exiting the object) we simply take an arbitrary value "Refraction Distance" of how far the ray will go after exiting the object then we reproject the position to screen space.

### Recap :
first intersection = positionWS</br>
second intersection = positionWS + frontFaceRefractedRay * Thickness</br>
third intersection = positionWS + backFaceRefractedRay * Refraction Distance (arbitrary value)</br>

The final assumption we can use here is that most people don't knows how glass is supposed to look like so why caring that much about accuracy :)</br>
