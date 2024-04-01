shader_type canvas_item;

uniform sampler2D color_texture;


void fragment() {
	COLOR = texture(TEXTURE, UV) * texture(color_texture, UV);
}
