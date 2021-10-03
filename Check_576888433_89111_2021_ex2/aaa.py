import sys


def a():
    mult = 50
    num = int(sys.argv[1])
    print(f"The number is {num}*{mult}={num*mult}", end='')


a()
