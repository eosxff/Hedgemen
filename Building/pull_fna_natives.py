import sys
import os
import shutil
import subprocess
import urllib.request
import tarfile
import glob

from pathlib import Path

cwd = Path('tmp').parent.absolute()
print(f'Working from directory: {cwd}')

temporary_directory=f'{cwd}/tmp'
natives_directory=f'{cwd}/natives'

def copy_file(src, dst):
	shutil.copy(f'{temporary_directory}/{src}', f'{natives_directory}/{dst}')

archive_url='https://fna.flibitijibibo.com/archive/fnalibs.tar.bz2'
archive_download_file=f'{temporary_directory}/fna_natives.tar.bz2'

# create temporary directory then download the file there
Path(temporary_directory).mkdir(parents=True, exist_ok=True)
urllib.request.urlretrieve(archive_url, archive_download_file)

# extract the archive
archive = tarfile.open(archive_download_file)
archive.extractall(path=temporary_directory)
archive.close()

Path(natives_directory).mkdir(parents=True, exist_ok=True)
shutil.copytree(src=f'{temporary_directory}/vulkan', dst=f'{natives_directory}/vulkan', dirs_exist_ok=True)

copy_file('x64/FAudio.dll', 'FAudio.dll')
copy_file('x64/FNA3D.dll', 'FNA3D.dll')
copy_file('x64/libtheorafile.dll', 'libtheorafile.dll')
copy_file('x64/SDL2.dll', 'SDL2.dll')

copy_file('osx/libFAudio.0.dylib', 'libFAudio.dylib')
copy_file('osx/libFNA3D.0.dylib', 'libFNA3D.dylib')
copy_file('osx/libtheorafile.dylib', 'libtheorafile.dylib')
copy_file('osx/libSDL2-2.0.0.dylib', 'libSDL2.dylib')
copy_file('osx/libvulkan.1.dylib', 'libvulkan.dylib')

copy_file('lib64/libFAudio.so.0', 'libFAudio.so')
copy_file('lib64/libFNA3D.so.0', 'libFNA3D.so')
copy_file('lib64/libtheorafile.so', 'libtheorafile.so')
copy_file('lib64/libSDL2-2.0.so.0', 'libSDL2.so')

shutil.rmtree(temporary_directory, ignore_errors=True)
#shutil.rmtree(natives_directory, ignore_errors=False)