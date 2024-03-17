
# simple script that generates all the serializable classes for JsonManifest

data_types = [ 'bool', 'int', 'long', 'float', 'double', 'object', 'string', 'NamespacedString', 'JsonManifest',
	'JsonElement' ]

collections_type0 = [ ]
collections_type1 = [ 'List', 'HashSet' ]
collections_type2 = [ 'Dictionary' ]

generated_string = ''

for data_type in data_types:
	generated_string += f'[JsonSerializable(typeof({data_type}))]\n'

for collection in collections_type0:
	f'[JsonSerializable(typeof({collection}))]\n'

for collection in collections_type1:
	for data_type in data_types:
		generated_string += f'[JsonSerializable(typeof({collection}<{data_type}>))]\n'

for collection in collections_type2:
	for data_type1 in data_types:

		if collection == 'Dictionary':
			if data_type1 == 'JsonElement' or data_type1 == 'JsonManifest':
				continue

		for data_type2 in data_types:
			generated_string += f'[JsonSerializable(typeof({collection}<{data_type1}, {data_type2}>))]\n'

file = open('generated_attributes.txt', 'w')
file.write(generated_string)
file.close()
