import random

def check_element_existence(objectDict, element1, element2):
    for x, y in objectDict.items():
        if(element1 == x or element2 == y):
            return True
    return False

def bubbleSort(array):
  
  for i in range(len(array)):
    for j in range(0, len(array) - i - 1):
      if array[j] < array[j + 1]:
        temp = array[j]
        array[j] = array[j+1]
        array[j+1] = temp

  return array


def mergeSort(array):
    if len(array) > 1:
        r = len(array)//2
        L = array[:r]
        M = array[r:]

        mergeSort(L)
        mergeSort(M)

        i = j = k = 0

        while i < len(L) and j < len(M):
            if L[i] > M[j]:
                array[k] = L[i]
                i += 1
            else:
                array[k] = M[j]
                j += 1
            k += 1

        while i < len(L):
            array[k] = L[i]
            i += 1
            k += 1

        while j < len(M):
            array[k] = M[j]
            j += 1
            k += 1
    
    return array

def compile_profit_weight_ratio(objectDict, n):
    objectArr = []
    i = 0
    for weight, (profit, profit_weight_ratio) in objectDict.items():
        objectArr.append(profit_weight_ratio)  # Append profit_weight_ratio to objectArr
        i += 1
        if i >= n:
            break

    return objectArr

# def rearrange_sorted_objects(objectsDict, sorted_PW_ratio):
    sorted_objectsDict = {}
    for ratio in sorted_PW_ratio:
        for weight, (profit, profit_weight_ratio) in objectsDict.items():
            if ratio == profit_weight_ratio:
                sorted_objectsDict[weight] = (profit, profit_weight_ratio)
            else:
                continue

    return sorted_objectsDict
   

def rearrange_sorted_objects(objectsDict, sorted_PW_ratio):
    sorted_objectsDict = {}

    # Convert the profit-weight ratios into a sorted list
    ratios = sorted(objectsDict.values(), key=lambda x: x[1])

    for ratio in sorted_PW_ratio:
        
        index = binarySearch(ratios, ratio, 0, len(ratios) - 1)
        if index != -1:
            weight = list(objectsDict.keys())[list(objectsDict.values()).index(ratios[index])]
            sorted_objectsDict[weight] = objectsDict[weight]

    return sorted_objectsDict

def binarySearch(array, x, low, high):
    if high >= low:
        mid = low + (high - low)//2
        if array[mid][1] == x:
            return mid
        elif array[mid][1] > x:
            return binarySearch(array, x, low, mid-1)
        else:
            return binarySearch(array, x, mid + 1, high)
    else:
        return -1


def compute_profit(sorted_objects, m):
    profit_table = {
        # weight : profit, profit-weight ratio, x(is w < m ? 1 : m/w), m(m - w * x), profit-per-weight(Pi * x)
    }
    current_carrying_capacity = m
    current_profit_per_weight = 0

    for weight, (profit, profit_weight_ratio) in sorted_objects.items():
        if weight < current_carrying_capacity :
            x = 1
        else:
            x = round(current_carrying_capacity/weight, 2)
        
        current_carrying_capacity = round(((current_carrying_capacity - weight) * x), 2)
        current_profit_per_weight = profit * x

        if current_carrying_capacity <= 0: break
        else:
            profit_table[weight] = (profit, profit_weight_ratio, x, current_carrying_capacity, current_profit_per_weight)
        

    for w, (p, pw, xf, mf, pf) in profit_table.items():
        print(w, ": ", p, ", ", pw, ", ", xf, ", ", mf, ", ", pf)

    weight_sum = 0
    for w, (p, pw, xf, mf, pf) in profit_table.items():
        weight_sum += w

    print("Carrying Capacity: 200")
    print("Total Weights: ", weight_sum)
        
        
def generate_objects(m, n): # m = carrying capacity; n = no. of objects
    objectsDict = {
    } # weight : profit, profit-weight ratio
    
    i = 0
    while i < n:
        status = True
        while status:
            weight = random.randint(1, 100)
            profit = random.randint(1, 100)

            if check_element_existence(objectsDict, weight, profit) is False:
                profit_weight_ratio = round(profit/weight, 2)
                status = False
        
        objectsDict[weight] = (profit, profit_weight_ratio)
        i += 1

    sorted_PW_ratio = mergeSort(compile_profit_weight_ratio(objectsDict, n))    
    sorted_objects = rearrange_sorted_objects(objectsDict, sorted_PW_ratio)
    compute_profit(sorted_objects, m)
    

generate_objects(1000, 100)




