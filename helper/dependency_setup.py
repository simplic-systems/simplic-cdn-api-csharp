# -----------------------------------------------------------------------
# Prepare visual studio dependencies before compiling
# -----------------------------------------------------------------------

# Imports
from distutils.dir_util import copy_tree
import os

base_path = os.path.dirname(os.path.realpath(__file__)) + '\\..\\'

# copy subdirectory example
dependencyDirectory = base_path + "\\dependencies"
debugDirectory = base_path + "\\src\\Simplic.CDN.CSharp\\bin\\Debug"
releaseDirectory = base_path + "\\src\\Simplic.CDN.CSharp\\bin\\Release"

# Copy files to release and debug directory for compiling
copy_tree(dependencyDirectory, debugDirectory)
copy_tree(dependencyDirectory, releaseDirectory)