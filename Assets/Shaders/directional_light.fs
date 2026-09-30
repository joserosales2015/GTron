#version 330

in vec2 fragTexCoord;
in vec4 fragColor;
in vec3 fragNormal;

uniform sampler2D texture0;
uniform vec4 colDiffuse;

uniform vec3 lightDirection;
uniform vec4 lightColor;
uniform vec4 ambientColor;

out vec4 finalColor;

void main()
{
    vec4 baseColor =
        texture(texture0, fragTexCoord) *
        colDiffuse *
        fragColor;

    vec3 normal = normalize(fragNormal);

    // lightDirection representa la dirección en que viaja la luz.
    float diffuse = max(
        dot(normal, normalize(-lightDirection)),
        0.0);

    vec3 illumination =
        ambientColor.rgb +
        lightColor.rgb * diffuse;

    finalColor = vec4(
        baseColor.rgb * illumination,
        baseColor.a);
}