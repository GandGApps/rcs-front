# -*- coding: utf-8 -*-

from token import OP
from lxml import etree
import re

def replace_static_resource(xml_content):
    # ������ XML � ���������� ���������� � ������������� ����
    namespaces = {'x': 'http://schemas.microsoft.com/winfx/2006/xaml'}
    root = etree.fromstring(xml_content.encode('utf-8'))

    # �������� ������� ��� ������������� ������ �������� � �� �������
    color_dict = {}
    for child in root:
        if isinstance(child.tag, str) and child.tag.endswith('SolidColorBrush') and '{http://schemas.microsoft.com/winfx/2006/xaml}Key' in child.attrib:
            color_dict[child.attrib['{http://schemas.microsoft.com/winfx/2006/xaml}Key']] = child.attrib['Color']

    # ������ StaticResource �� SolidColorBrush
    for static_resource in root.xpath(".//*[local-name() = 'StaticResource']", namespaces=namespaces):
        key = static_resource.attrib['ResourceKey']
        if key in color_dict:
            new_element = etree.Element('{http://schemas.microsoft.com/winfx/2006/xaml/presentation}SolidColorBrush')
            new_element.attrib['{http://schemas.microsoft.com/winfx/2006/xaml}Key'] = static_resource.attrib['{http://schemas.microsoft.com/winfx/2006/xaml}Key']
            new_element.attrib['Color'] = color_dict[key]
            static_resource.getparent().replace(static_resource, new_element)

    # ����������� ������������ XML
    return etree.tostring(root, pretty_print=True, encoding='unicode')

with open('Light.xaml', 'r') as file:
    lines = file.readlines()
    
    filtered_lines = [line for line in lines if "<!-- This Light.xaml file serves as a template." not in line]
    xml_content = "".join(filtered_lines)

    xml_content = replace_static_resource(xml_content)
    
with open('LightGenerated.xaml', 'w') as file:
    file.write(xml_content)