#version 330

in vec3 fragNormal;

uniform vec4 colDiffuse;

out vec4 finalColor;

void main()
{
    vec3 normal = normalize(fragNormal);
    vec3 lightDirection = normalize(vec3(-0.5, 1.0, 0.3));

    float diffuse = max(dot(normal, lightDirection), 0.0);
    float brightness = 0.25 + diffuse * 0.75;

    finalColor = vec4(colDiffuse.rgb * brightness, 1.0);
}