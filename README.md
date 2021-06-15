# StockApi
## Summary
#### Provide some APIs for obtaining stock data from [TWSE](https://www.twse.com.tw/zh/page/trading/exchange/BWIBBU_d.html), including P/E ratio, P/B ratio and dividend yield.
## Usage
#### Retrieve the stock data by stock number for the current n days.
* GET https://localhost:5001/api/stock/number/{stockNo}
* Path parameter
  * stockNo<sup>*</sup>: The stock number of stock you want to search.
* Query parameter
  * n: Current n days of data will be returned. (default 10)
##
#### Retrieve stocks of given date and sorted by given field.
* GET https://localhost:5001/api/stock/date/{date}
* Path parameter
  * date<sup>*</sup>: The date of data you want to search. (YYYYMMDD)
* Query parameter
  * n: The number of data to display. (default 10)
  * sortBy: Sorting the data by the given field. (default PER)
    * available value
      * PER: P/E ratio
      * PBR: P/B ratio
      * DY: Dividend Yield
  * reverse: Reverse order or not. (default false)
##
#### Between the given date range, find the longest interval that the given field data is increasing.
* GET https://localhost:5001/api/stock/number/{stockNo}/interval/{field}
* Path parameter
  * stockNo<sup>*</sup>: The stock number of stock you want to search.
  * field<sup>*</sup>: The field of data you want to search.
    * available value
      * PER: P/E ratio
      * PBR: P/B ratio
      * DY: Dividend Yield
* Query parameter
  * from<sup>*</sup>: The first date of the date range. (YYYYMMDD)
  * to<sup>*</sup>: The last date of the date range. (YYYYMMDD)
  * distinct: Consider distinctly or not. (default true)

##### <sup>*</sup>: required parameter
