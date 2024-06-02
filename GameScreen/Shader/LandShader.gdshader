shader_type canvas_item;

uniform sampler2D color_texture;

const float hex_width = 148.0;
const float hex_height = 129.0;
const vec2 hex_size = vec2(hex_width, hex_height);

// 用于计算六边形的一些常数和辅助函数，包括边距和角度
const float sqrt3 = sqrt(3.0);
const float hex_radius = hex_width / sqrt3;
const float hex_a = hex_radius / 2.0;
const float hex_b = hex_height / 2.0;
const vec2 hex_dx = vec2(hex_width * 0.75, 0.0);
const vec2 hex_dy = vec2(hex_width * 0.25, hex_height * 0.5);

void fragment() {

	COLOR = texture(TEXTURE, UV) * texture(color_texture, UV);
}
