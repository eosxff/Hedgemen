import requests
import os
import tarfile

def download_fna_natives(url, filename):
    response = requests.get(url, stream=True)

    if(response.status_code == 200):
        if not os.path.isdir('tmp'):
            os.mkdir('tmp')

        with open(f'tmp/{filename}', 'wb') as f:
                f.write(response.raw.read())

def extract_fna_natives(filename):
    file = tarfile.open(f'tmp/{filename}')
    file.extractall

url = 'https://fna.flibitijibibo.com/archive/fnalibs.tar.bz2'
fna_natives_filename = 'fna_natives.tar.bz2'

download_fna_natives(url, fna_natives_filename)