extends MeshInstance3D


func _ready():
	var sphere_mesh = mesh.get_radial_segments ( )
	print(sphere_mesh)



