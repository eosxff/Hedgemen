import sys
import os
import subprocess
import urllib.request
from distutils.dir_util import copy_tree
from shutil import rmtree

resources_directory='resources'

def get_self_contained():
	return '--self-contained' in sys.argv

def get_csproject():
	if '--project' or '-p' in sys.argv:
		flag_index = -1
		value_index = sys.argv.index
		return 

def get_configuration():
	if '--release' or '-r' in sys.argv:
		return 'release'
	if '--debug' or '-d' in sys.argv:
		return 'debug'
	else:
		print('Please specify a build type using --debug or --release.')
		exit

def get_rid(): # maybe return array of rids?
	if '--windows' in sys.argv:
		return "win-x64"
	if '--linux ' in sys.argv:
		return 'linux-x64'
	if '--osx' in sys.argv:
		return 'osx-x64'
	else:
		return ''

def get_output_directory():
	flag_index = -1
	
	if '-o' in sys.argv:
		flag_index = sys.argv.index('-o')
	elif '--output' in sys.argv:
		flag_index = sys.argv.index('--output')
	else:
		print('Please specify an output directory using --output or -o.')
		exit
	
	if flag_index != -1 and len(sys.argv) > flag_index + 1:
		return sys.argv[flag_index + 1]
	else:
		print('There must be an argument after %s.', sys.argv[flag_index])
		exit

def create_directory(directory):
	os.makedirs(directory, exist_ok=True)
	
	if not os.path.exists(directory):
		print('Failed to create directory at %s.', directory)
		exit

def publish_csproj(csproj_directory_local, build_configuration, build_rid, is_self_contained, output_directory):

	arguments = [ 'dotnet', 'publish', csproj_directory_local ]

	if build_rid:
		arguments.append('-r')
		arguments.append(build_rid)

	arguments.append('-c')
	arguments.append(build_configuration)

	arguments.append('-o')
	arguments.append(output_directory)

	if is_self_contained:
		arguments.append('--self-contained')
		arguments.append('/p:PublishSingleFile=true')
	else:
		arguments.append('--no-self-contained')

	exit_code = subprocess.call(arguments, stdout=subprocess.DEVNULL)
	
	if(exit_code == 0):
		print(f'Successfully published {csproj_directory_local}.')
	else:
		print(f'Publish on {csproj_directory_local} failed with exit code {exit_code}')
	

def make_build():
	if not os.path.exists(resources_directory):
		print('Could not find directory at %s.', resources_directory)
		exit

	is_self_contained = get_self_contained()
	build_configuration = get_configuration()
	build_rid = get_rid()

	output_directory = f'./{get_output_directory()}'
 
	print(f'Creating build at {os.path.abspath(output_directory)}.')

	create_directory(output_directory)

	publish_csproj(
		'../Hedgemen/Hedgemen.csproj',
		build_configuration,
		build_rid,
		is_self_contained,
		f'./tmp/{output_directory}/hedgemen')
	
	publish_csproj(
		'../ExampleMod/ExampleMod.csproj',
		build_configuration, 
		build_rid,
		False,
		f'./tmp/{output_directory}/example_mod')
	
	#copy_tree(resources_directory, output_directory)
	#rmtree('./tmp')

make_build()