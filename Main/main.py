import csv

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

def binarySearch(array, x, low, high):
    if high >= low:
        mid = low + (high - low)//2
        if array[mid][2] == x:  # Compare with profit_weight_ratio
            return mid
        elif array[mid][2] > x:
            return binarySearch(array, x, low, mid-1)
        else:
            return binarySearch(array, x, mid + 1, high)
    else:
        return -1

def tabulated_elements():
    objectsDict = {
        # itemID(0) : (weight(2), profit(7), profit-weight ratio) tuple-based
    }

    with open('Data_Sheet3.csv') as csvfile:
        csv_reader = csv.reader(csvfile)
        next(csv_reader)

        for row in csv_reader:
            itemID = row[0]
            weight = float(row[2])
            profit = float(row[7])
            profit_weight_ratio = round(profit/weight, 2)
            objectsDict[itemID] = (weight, profit, profit_weight_ratio)
    
    # for id, (x, y, z) in objectsDict.items():
    #     print(id, ": ", x, ", ", y, ", ", z)

    return objectsDict

    
def compiled_profit_weight_ratio(objectDict, n):
    objectArr = [profit_weight_ratio for _, (_, _, profit_weight_ratio) in objectDict.items()][:n]
    return objectArr


def rearrange_sorted_objects(objectsDict, sorted_PW_ratio):
    sorted_objectsDict = {}

    # Convert the profit-weight ratios into a sorted list
    ratios = sorted(objectsDict.values(), key=lambda x: x[1])

    for ratio in sorted_PW_ratio:
        # Perform binary search to find the index of ratio in ratios
        index = binarySearch(ratios, ratio, 0, len(ratios) - 1)
        
        # If found, retrieve the corresponding weight from objectsDict
        if index != -1:
            weight = list(objectsDict.keys())[list(objectsDict.values()).index(ratios[index])]
            sorted_objectsDict[weight] = objectsDict[weight]

    return sorted_objectsDict


def compute_total_profit(sortedObjectsDict, m):
    profit_table = {
        # id : weight, profit, profit-weight ratio, x(is w < m ? 1 : m/w), m(m - w * x), profit-per-weight(Pi * x)
    }

    current_carrying_capacity = m

    for itemID, (weight, profit, profit_weight_ratio) in sortedObjectsDict.items():
        if weight < current_carrying_capacity:
            x = 1
        else:
            x = round(current_carrying_capacity/weight, 2)
        
        current_carrying_capacity = round((current_carrying_capacity - weight) * x, 2)
        current_profit_per_weight = round(profit * x, 2)

        if current_carrying_capacity <= 0: break
        else:
            profit_table[itemID] = (weight, profit, profit_weight_ratio, x, current_carrying_capacity, current_profit_per_weight)
        
    
    return profit_table


def main():
    m = 500
    n = 96
    
    objectsDict = tabulated_elements()
    pwRatioArr = compiled_profit_weight_ratio(objectsDict, n)
    sorted_pwRatioArr = mergeSort(pwRatioArr)
    sorted_objectsDict = rearrange_sorted_objects(objectsDict, sorted_pwRatioArr)
    
    total_profit = 0
    total_weights = 0
    calculated_profit = compute_total_profit(sorted_objectsDict, m)
    for id, (weight, profit, profit_weight_ratio, x, current_carrying_capacity, current_profit_per_weight) in calculated_profit.items():
        print(id, ": ", weight, ", ", profit, ", ", profit_weight_ratio, ", ", x, ", ", current_carrying_capacity, ", ", current_profit_per_weight)
        total_profit += current_profit_per_weight
        total_weights += weight
    

    print(f"\nCarrying Capacity: {m}")
    print(f"Total No of items: {n}")
    print(f"No. of Accomodated Items: {len(calculated_profit)}")
    print(f"Total Weights Accomodated: {total_weights}")
    print(f"Total Logistics Profit Gained: {total_profit}")


main()


