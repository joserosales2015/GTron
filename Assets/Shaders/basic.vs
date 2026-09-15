#version 330

in vec3 vertexPosition;
in vec3 vertexNormal;
in mat4 instanceTransform;

uniform mat4 mvp;

out vec3 fragNormal;

void main()
{
    fragNormal =
        mat3(instanceTransform) * vertexNormal;

    gl_Position =
        mvp *
        instanceTransform *
        vec4(vertexPosition, 1.0);
}