/*import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { TextInput } from '@playcanvas/pcui/pcui-react.js';

ReactDOM.render(
    <TextInput />,
    document.getElementById('renderer')
);*/

import React, { useState, useEffect } from 'react';
import ReactDOM from 'react-dom';
import { Container, TextInput, BooleanInput, Label, SelectInput, Button, Panel }from '@playcanvas/pcui/pcui-react.js';
import { Observer, BindingTwoWay } from '@playcanvas/pcui/pcui-binding.js';

const observer = new Observer({ input: '', items: {} });

export const TodoList = (props: any) => {
    useEffect(() => { document.body.style.backgroundColor = '#374346' }, [])
    const [ items, setItems ] = useState({});
    const [ listFilter, setListFilter ] = useState(0);
    observer.on('items:set', setItems);
    const addItem = (value: string) => {
        const items = observer.get('items');
        if (value === '') return;
        items[Date.now()] = { done: false, text: value };
        observer.set('input', '');
        observer.set('items', items);
    };
    const removeItem = (key: string) => {
        const items = observer.get('items');
        delete items[key];
        observer.set('items', items);
    };
    const toggleItem = (key: string) => {
        const items = observer.get('items');
        items[key].done = !items[key].done;
        observer.set('items', items);
    };
    const textInputLink = { observer, path: 'input' };
    return (
        <Panel
        collapsible
        headerText="TODO List"
        >
            <Container class='todo'>
                <TextInput blurOnEnter={false} placeholder='enter item' binding={new BindingTwoWay()} link={textInputLink} onChange={addItem}/>
                <SelectInput type="number" options={[{v: 0, t: 'Show all items'}, {v: 1, t: 'Show active items'}, {v: 2, t: 'Show done items'}]} onChange={setListFilter} />
                <Container>
                    {Object.keys(items).map(key => {
                        var item = items[key];
                        if (listFilter !== 0) {
                            if ((listFilter === 1 && item.done) || (listFilter === 2 && !item.done)) return null;
                        }
                        return [
                            <Container key={key} class={'todo-item'}>
                                <BooleanInput onChange={() => toggleItem(key)} value={item.done} />
                                <Label text={item.text}/>
                                <Button icon='E124' text='' size='small' onClick={() => removeItem(key)} />
                            </Container>
                        ];
                    })}
                </Container>
            </Container>
        </Panel>
    );
};

ReactDOM.render(<TodoList />, document.body);