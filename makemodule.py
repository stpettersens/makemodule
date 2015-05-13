#!/bin/env python
"""
makemodule

Module generation tool

Copyright (c) 2015 Sam Saint-Pettersen.
Released under the MIT/X11 License.
"""
import sys
import os
import xml.dom.minidom as xml

class makemodule:

    def __init__(self, args):
        if len(args) == 1:
            self.displayUsage()
        else:
            self.writeModuleXML()

    def displayUsage(self):
        print(__doc__)
        print('Usage: makemodule [module..module]\n')
        sys.exit(1)

    def writeModuleXML(self):
        names = []
        enabled = []
        redirect = ''
        cleanup = False

        if os.name == 'nt':
            redirect = ' > a.tmp 2>&1'
            cleanup = True
        else:
            redirect = ' >> /dev/null 2>&1'

        for arg in sys.argv[1:]:
            names.append(arg)
            exitCode = int(os.system(arg + redirect))
            if exitCode == 32512:
                enabled.append(False)
            else:
                enabled.append(True)

        doc = xml.Document()
        c = doc.createElement('configuration')
        doc.appendChild(c)

        i = 0
        for name in names:
            m = doc.createElement('module')
            c.appendChild(m)
            n = doc.createElement('name')
            m.appendChild(n)
            n_is = doc.createTextNode(name)
            n.appendChild(n_is)
            e = doc.createElement('enabled')
            m.appendChild(e)
            e_is = doc.createTextNode(str(enabled[i]))
            e.appendChild(e_is)
            i = i + 1

        print('Writing modules.xml...')
        f = open('modules.xml', 'w')
        f.write(doc.toprettyxml())
        f.close()

        if os.name == 'nt': os.remove('a.tmp')

makemodule(sys.argv)
