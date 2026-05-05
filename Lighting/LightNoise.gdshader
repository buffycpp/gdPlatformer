shader_type canvas_item;

uniform sampler2D noise_tex;
uniform float strength = 0.1;
uniform float speed = 1.0;

void fragment() {
    vec2 uv = UV;

    float noise = texture(noise_tex, uv + TIME * speed).r;

    uv += (noise - 0.5) * strength;

    COLOR = texture(TEXTURE, uv);
}