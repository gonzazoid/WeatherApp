/// <reference path="./tsd.d.ts" />
import { ajaxRowOptions, ajaxGridOptions } from './jqGridOptions';
{
const table = $('#locations_list');

const getParameterByName = function (name: string, url?: string) {
    if (!url){
        url = window.location.href;
    }
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results){
        return null;
    }
    if (!results[2]){
        return '';
    }
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
};

const pad = function (i: number): string {
    return i < 10 ? '0' + String(i): String(i);
};

const dateFormatter = function (cellValue: number, opts: any, rwd: any) {
    const date = new Date(cellValue*1000);
    return `${pad(date.getDate())}/${pad(date.getMonth() + 1)}/${date.getFullYear()} ${pad(date.getHours())}:${pad(date.getMinutes())}`;
};

const temperatureFormatter = function (cellValue: number, opts: any, rwd: any) {
    return `${(cellValue - 273.15).toFixed(2)}&deg;C`;
};

const pressureFormatter = function(cellValue: number, opts: any, rwd: any){
    return '' + (cellValue).toFixed(2) + '&#x33D4';
};

(table.jqGrid as any)({
    url:'/api/getHistory',
    datatype: 'json',
    serializeGridData: function(postData: any) {
        postData.placeId = getParameterByName('placeId');
        return JSON.stringify(postData);
    },
    ajaxGridOptions,
    ajaxRowOptions,
    colNames:['time', 'open weather pressure', 'open weather temp', 'yahoo pressure', 'yahoo temp'],
    colModel:[
        {name:'stamp',index:'stamp', width:120, formatter: dateFormatter}
       ,{name:'openWeatherPressure', width:150, sortable: false, editable: false, formatter: pressureFormatter}
       ,{name:'openWeatherTemp', width:130, sortable: false, editable: false, formatter: temperatureFormatter}
       ,{name:'yahooPressure', width:120, sortable: false, editable: false, formatter: pressureFormatter}
       ,{name:'yahooTemp', width:100, sortable: false, editable: false, formatter: temperatureFormatter}
    ],
    beforeProcessing: function(data: any){
        if(data.locationName){
            const newCaption = 'weather history in the area: ' + data.locationName;
            table.jqGrid('setCaption', newCaption);
        }
    },
    rowNum:10,
    rowList:[10,20,30],
    pager: '#pager',
    sortname: 'stamp',
    viewrecords: true,
    sortorder: 'asc',
    caption:'weather history in the area: '
});

table.jqGrid('inlineNav','#pager');
}
