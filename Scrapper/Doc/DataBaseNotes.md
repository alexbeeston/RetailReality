## Product Fields
- Product ID
- Name
- Brand
- Department
- Gender
- Product
- Silhouette
- [Date added to DB?] so that we can know how long we've been tracking this item? Will it conflict with days since DA in the offer table?

## Offer Fields
- Offer ID
- Product ID (gender, product, silhouette, etc., stored in the Products table)
- Year of offer (UTC)
- Month of offer (UTC)
- Date of offer (UTC)
- Hour of offer (UTC)
- Minute of offer (UTC)
- Price 1 LabelType
- Price 1 PriceType
- Price 1 Price
- Price 1 DerivedPrice ID (-1 if LabelType is Single)

- Price 2 LabelType
- Price 2 PriceType
- Price 2 Price
- Price 2 DerivedPrice ID (-1 if LabelType is Single)






label1 previously seen (not in composite)
        - 'Reg.'
        - 'Clearance'

price1 previously seen (not in composite)
        - '$x or x / $x'

price2 previously seen (not in composite)
        1. ''
        2. 'group $'
        3. 'Reg. $ - $'
        4. 'or Reg. $ each'
        5. 'Reg $'
        5. 'Original $ - $'
        7. '$'



