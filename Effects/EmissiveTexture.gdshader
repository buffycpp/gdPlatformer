shader_type canvas_item;

uniform sampler2D emission_mask;
uniform vec4 emission_color : source_color = vec4(1.0, 0.5, 0.1, 1.0);
uniform float emission_strength = 2.0;

void fragment() {
    vec4 base = texture(TEXTURE, UV);

    float mask = texture(emission_mask, UV).r;

    COLOR = base;

    // Add emissive glow
    COLOR.rgb += emission_color.rgb * mask * emission_strength;
}