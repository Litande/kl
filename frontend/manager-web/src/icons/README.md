We don't use SVG icons directly, we convert them to a icon font using icomoon.\
When we want to change, add or remove and icon in "svg-platform" we have to generate a new font files from icomoon.\
Here are the steps of creating icomoon font from SVG icons.

### Ensure that the required icon doesn't exist in the project.\ You can check that in **/icons** page in the platform (accessable through the sidemenu.

1. Add the newly added icons in **"svg-platform"** or respectively in **"svg-highchart"** folder.
2. Go to [Icomoon](https://icomoon.io/)
3. If you don't have an existing profile, register a new one (free plan is enough to get the job done).
4. Click on **"IcoMoon App"**.
5. Click on **"Import Icons"** and select all svg files from **"svg"** folder.
6. Arrange icons by **name** and **select all** icons from this set.
7. Click on **"Generate Font"** from the bottom of your screen.
8. Click on **"Preferences"** and choose the **Font-Name** to be **"tp-icomoon"**.
9. Download the font. It's going to be an archived folder with several files in it.
10. Get **"fonts"** folder and **"style.css"** file and place them in our **"icomoon"** folder.
