import Modal from 'react-modal';
import React, { Component } from 'react';
import { Button, Form } from 'react-bootstrap';

export class Inventory extends Component {
    static displayName = Inventory.name;

    constructor(props) {
        super(props);
        this.state = {
            modalItemOpened: false,
            modalItem: {},
            loading: true,
            filter: {
                computerType: "",
                brand: ""
            },
            inventory: [],
            error: {
                message: "", list: {}
            },
            lookupLists: { computerTypes: [], brands: [] }
        };

    }

    // load the grid data at start
    componentDidMount() {
        this.populateInventoryData(true);
    }

    async populateInventoryData(loadLookups) {
        loadLookups = loadLookups || false;
        var computerType = this.state.filter.computerType || "";
        var brand = this.state.filter.brand || "";
        const response = await fetch(`inventory/list?computerType=${computerType}&brand=${brand}&loadLookups=${loadLookups}`);
        var data = await response.json();
        if (data.lookupLists) this.setState({ lookupLists: data.lookupLists });
        this.setState({ inventory: data.inventory, loading: false });
    }

    // we refresh the grid when filter changed
    filterChanged = (prop, value) => {
        var filter = this.state.filter;
        filter[prop] = value;
        this.setState({ filter: filter }, () => {
            this.populateInventoryData(false);
        });
    };

    // clear the filter, and refresh the grid 
    clearFilter = e => {
        this.setState({ filter: { brand: "", computerType: "" } }, () => {
            this.populateInventoryData(false);
        });
    };

    // when popup values are changed, we have to update the state. Rather than creating change events for 
    // all form elements, we use one handler
    modalItemChanged = (prop, value) => {
        var modalItem = this.state.modalItem;
        modalItem[prop] = value;
        this.setState({ modalItem: modalItem });
    };

    // when features are changed, we have to update the state. Rather than creating change events for 
    // all form elements, we use one handler
    featureChanged = (prop, value) => {
        var features = this.state.modalItem.features;
        for (let i = 0; i < features.length; i++)
            if (features[i].featureName == prop)
                features[i].featureValue = value;
        let modalItem = this.state.modalItem;
        modalItem.features = features;
        this.setState({ modalItem: modalItem});
    };

    // save one item to the server
    async saveOne() {
        const params = {
            headers: {
                'Accept': "application/json, text/plain, */*",
                'Content-Type': "application/json;charset=utf-8"
            },
            body: JSON.stringify(this.state.modalItem),
            method: "POST"
        };

        const response = await fetch(`inventory/save`, params);
        var result = await response.json();

        if (result.code == 1) {
            // server call success
            this.setState({ modalItemOpened: false, modalItem: {}, error: { message: "", list: {}} });
            this.populateInventoryData(false);
        }
        else {
            // server call failure
            var list = {};
            result.data.forEach((v, i) => {
                list[v.memberNames[0]] = v.errorMessage;
            });
            this.setState({ error: { message: result.message, list: list } });

        }
        return false;

    }

    // find one item from the server
    async fetchOne(id) {
        const response = await fetch(`inventory/one?id=${id}`, { method: "POST" });
        var result = await response.json();

        if (result.code == 1) {
            // server call success
            this.setState({ modalItemOpened: true, modalItem: result.data });
        }
        else {
            // server call failure
            alert(result.message)
        }
        return false;

    }

    render() {
        return (

            <div>
                <h1 id="tabelLabel" >Inventory</h1>
                <div className="search-filter form-inline pb-1">
                    <div className="form-group">
                        <label className="control-label pr-4">Type:</label>
                        <div className="">
                            <select className="form-control" onChange={(e) => this.filterChanged("computerType", e.target.value)}>
                                <option value="">[select]</option>
                                {
                                    this.state.lookupLists.computerTypes.map((ctype, index) =>
                                        <option key={index}>{ctype}</option>
                                    )
                                }
                            </select>
                        </div>
                    </div>
                    <div className="form-group">&nbsp;</div>
                    <div className="form-group">
                        <label className="control-label pl-6 pr-4">Brand:</label>
                        <div className="">
                            <select className="form-control" onChange={(e) => this.filterChanged("brand", e.target.value)}>
                                <option value="">[select]</option>
                                {
                                    this.state.lookupLists.brands.map((brand, index) =>
                                        <option key={index}>{brand}</option>
                                    )
                                }
                            </select>
                        </div>
                    </div>
                    <div className="form-group">
                        <label className="control-label">&nbsp;</label>
                        <div>
                            <button className="btn btn-success ml-1" onClick={this.clearFilter}>Clear</button>
                        </div>
                    </div>
                </div>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Brand</th>
                            <th>Type</th>
                            <th>Processor</th>
                            <th>Price</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            this.state.loading ? (
                                <tr key="1">
                                    <td colSpan="5">Loading...</td>
                                </tr>) :
                                this.state.inventory.length > 0 ?
                                    (
                                        this.state.inventory.map((inventory, index) =>
                                            <tr key={inventory.id}>
                                                <td><button style={{ border: "none", backgroundColor: "transparent" }} onClick={(e) => this.fetchOne(inventory.id)}><u>{inventory.id}</u></button></td>
                                                <td>{inventory.brand}</td>
                                                <td>{inventory.computerType}</td>
                                                <td>{inventory.processor}</td>
                                                <td>{inventory.price}</td>
                                            </tr>

                                        )
                                    ) :
                                    (
                                        <tr key="1">
                                            <td colSpan="5">There are no computers matching your cliteria</td>
                                        </tr>
                                    )
                        }
                    </tbody>
                </table>
                <div>
                    <button className="btn btn-warning mr-1" onClick={() => this.fetchOne(0)}>Add Item</button>
                    <button className="btn btn-warning" onClick={() => this.fetchOne(1000)}>Find Invalid Item</button>
                </div>
                <Modal
                    size="lg"
                    isOpen={this.state.modalItemOpened}
                    appElement={document.getElementById("root")} >
                    <h3>Inventory Item - {this.state.modalItem.id > 0 ? this.state.modalItem.id : "Add"}</h3>
                    <form>
                        <div className="error message" style={{ display: (this.state.error.message ? "block" : "none") }}>{this.state.error.message}</div>
                        <div className="row">
                            <div className="form-group col-sm-6">
                                <label htmlFor="brand">Brand</label>
                                <select className="form-control" id="computerType" value={this.state.modalItem.brand} onChange={(e) => { this.modalItemChanged("brand", e.target.value) }}>
                                    <option value="">[Select]</option>
                                    {
                                        this.state.lookupLists.brands.map((ctype, index) =>
                                            <option key={index}>{ctype}</option>
                                        )
                                    }
                                </select>
                                <div className="error brand" style={{display: (this.state.error.list["brand"] ? "block" : "none") }}>{ this.state.error.list["brand"]}</div>
                            </div>
                            <div className="form-group col-sm-6">
                                <label htmlFor="computerType">Computer Type</label>
                                <select className="form-control" id="computerType" value={this.state.modalItem.computerType} onChange={(e) => { this.modalItemChanged("computerType", e.target.value) }}>
                                    <option value="">[Select]</option>
                                    {
                                        this.state.lookupLists.computerTypes.map((ctype, index) =>
                                            <option key={index}>{ctype}</option>
                                        )
                                    }
                                </select>
                                <div className="error computerType" style={{ display: (this.state.error.list["computerType"] ? "block" : "none") }}>{this.state.error.list["computerType"]}</div>
                            </div>
                        </div>
                        <div className="row">
                            <div className="form-group col-sm-6">
                                <label htmlFor="processor">Processor</label>
                                <input type="text" className="form-control" id="processor" value={this.state.modalItem.processor} onChange={(e) => { this.modalItemChanged("processor", e.target.value) }} />
                            </div>
                            <div className="form-group col-sm-6">
                                <label htmlFor="price">Price</label>
                                <input type="number" className="form-control" id="price" value={this.state.modalItem.price} onChange={(e) => { this.modalItemChanged("price", e.target.value) }} />
                            </div>
                        </div>
                        <div className="row">
                            <div className="form-group col-sm-6">
                                <label htmlFor="noOfUSBPorts">USB Ports #</label>
                                <input type="text" className="form-control" id="noOfUSBPorts" value={this.state.modalItem.noOfUSBPorts} onChange={(e) => { this.modalItemChanged("noOfUSBPorts", e.target.value) }} />
                            </div>
                            <div className="form-group col-sm-6">
                                <label htmlFor="noOfRAMPorts">RAM Ports #</label>
                                <input type="text" className="form-control" id="noOfRAMPorts" value={this.state.modalItem.noOfRAMPorts} onChange={(e) => { this.modalItemChanged("noOfRAMPorts", e.target.value) }} />
                            </div>
                        </div>
                        <div className="form-group ">
                            <label htmlFor="features">Features</label>
                            <div className="p-2" style={{backgroundColor:"#efefef"} } >
                                <table style={{ width: "100%" }}>
                                    <tbody>
                                        {
                                            (this.state.modalItem.features) ?
                                                this.state.modalItem.features.map((feature, index) =>
                                                    <tr key={index}>
                                                        <td><i>{feature.featureName}</i></td>
                                                        <td><input className="form-control" type="text" id={`features${index}`} value={feature.featureValue} onChange={(e) => { this.featureChanged(feature.featureName, e.target.value) }} /></td>
                                                    </tr>
                                                ) :
                                                (<tr key="1">
                                                    <td colSpan="2">No features</td>
                                                </tr>)
                                        }

                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div className="form-group text-right">
                            <button className="btn btn-info mr-1" onClick={(e) => { this.saveOne(); e.preventDefault(); return false; }}>Save</button>
                            <button className="btn btn-warning" onClick={() => { this.setState({ modalItemOpened: false, error: { message: "", list: {}} }) }}>Close</button>
                        </div>
                    </form>
                </Modal>
            </div>
        );
    }
}