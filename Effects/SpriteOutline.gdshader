shader_type canvas_item;

uniform vec4 outline_color : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float outline_size = 1.0;

void fragment() {
    vec4 tex = texture(TEXTURE, UV);

    float alpha = tex.a;

    vec2 pixel_size = TEXTURE_PIXEL_SIZE * outline_size;

    // Check neighboring pixels
    float outline = 0.0;

    outline += texture(TEXTURE, UV + vec2(pixel_size.x, 0.0)).a;
    outline += texture(TEXTURE, UV + vec2(-pixel_size.x, 0.0)).a;
    outline += texture(TEXTURE, UV + vec2(0.0, pixel_size.y)).a;
    outline += texture(TEXTURE, UV + vec2(0.0, -pixel_size.y)).a;

    // Draw outline only outside the sprite
    if (alpha == 0.0 && outline > 0.0) {
        COLOR = outline_color;
    } else {
        COLOR = tex;
    }
}