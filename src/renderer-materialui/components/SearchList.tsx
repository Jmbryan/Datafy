import React, { useState, useEffect } from 'react';

const SearchList = () => {
  const [filter, setFilter] = useState("");
  const handleChange = (event: any) => setFilter(event.target.value);

  return (
    <div>
      <h2>Search List</h2>
      <input type="text" placeholder="Search..." value={filter} onChange={handleChange} />
      <ul>
        <li>Item</li>
        <li>Weapon</li>
        <li>FireMode</li>
      </ul>
    </div>
  );
};

export default SearchList;