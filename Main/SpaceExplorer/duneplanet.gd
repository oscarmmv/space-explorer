extends Node3D

var mesh_instance: MeshInstance3D = MeshInstance3D.new()
var sphere_mesh = SphereMesh.new()
var texture: ImageTexture

func _ready():
	var image = Image.new()
	var load_status = image.load("res://8k_venus_surface.jpg")
	if load_status == OK:
		texture = ImageTexture.new()
		texture.create_from_image(image)
		
		var material = ShaderMaterial.new()
		material.set_shader_parameter("albedo_texture", texture)
		
		mesh_instance.mesh = sphere_mesh
		add_child(mesh_instance)
		sphere_mesh.radius = 40.5
		sphere_mesh.height = 81
		sphere_mesh.radial_segments = 128
		sphere_mesh.rings = 64
		mesh_instance.set_surface_override_material(0, material)
	else:
		print("Failed to load texture")
