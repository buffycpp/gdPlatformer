shader_type canvas_item;

uniform float strength = 0.5;

uniform float radius = 4.0;        // max sampling distance in pixels
uniform float falloff = 2.0;       // higher = sharper AO
uniform float inner_strength = 1.0;
uniform float outer_strength = 0.5;

void fragment()
{
    float center = texture(TEXTURE, UV).a;
    vec2 px = TEXTURE_PIXEL_SIZE;

    float occ = 0.0;
    float w = 0.0;

    for (int x = -4; x <= 4; x++)
    {
        for (int y = -4; y <= 4; y++)
        {
            if (x == 0 && y == 0) continue;

            vec2 offset_px = vec2(float(x), float(y));

            float dist = length(offset_px);

            if (dist > radius) continue;

            float sample = texture(TEXTURE, UV + offset_px * px).a;

            float norm_dist = dist / radius;

            float weight = 1.0 / pow(dist, falloff);

            // optional shaping: stronger near center, weaker far away
            weight *= mix(inner_strength, outer_strength, norm_dist);

            occ += sample * weight;
            w += weight;
        }
    }

    occ /= w;

    float shadow = (1.0 - center) * pow(occ, 1.5);

    COLOR.rgb = vec3(0.0);
    COLOR.a = shadow * strength;
}