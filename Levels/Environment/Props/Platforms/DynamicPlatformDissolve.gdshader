shader_type canvas_item;

uniform float dissolve_amount : hint_range(0.0, 1.0) = 0.0;

uniform sampler2D noise_texture : hint_default_white;

uniform float pixel_size : hint_range(1.0, 64.0) = 8.0;
uniform float noise_scale : hint_range(0.1, 50.0) = 4.0;
uniform float noise_strength : hint_range(0.0, 0.5) = 0.2;

void fragment()
{
    vec4 tex = texture(TEXTURE, UV);
    if (tex.a <= 0.0) discard;

    // Pixelated noise coordinates
    vec2 pixel_uv = floor(UV * pixel_size) / pixel_size;

    float noise = texture(noise_texture, pixel_uv * noise_scale).r;

    // Keep height stable
    float height = UV.y;

    // Add a tiny safety bias so we never "half hit" the top edge
    float threshold = dissolve_amount + (noise - 0.5) * noise_strength;

    // IMPORTANT FIX: use step() instead of if/discard
    float mask = step(threshold, height);

    COLOR = tex;
    COLOR.a *= mask;
}