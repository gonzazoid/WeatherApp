/// <reference path="./tsd.d.ts" />
import { ajaxRowOptions, ajaxGridOptions } from './jqGridOptions';
// TODO use some state lib
var current_provider = '';

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
        postData.provider = current_provider;
        postData.placeId = getParameterByName('placeId');
        return JSON.stringify(postData);
    },
    ajaxGridOptions,
    ajaxRowOptions,
    colNames:['time', 'pressure', 'temp'],
    colModel:[
        {name:'stamp',index:'stamp', width:120, formatter: dateFormatter}
       ,{name:'pressure', width:150, sortable: false, editable: false, formatter: pressureFormatter}
       ,{name:'temp', width:130, sortable: false, editable: false, formatter: temperatureFormatter}
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

// providers list
const select = document.getElementById('providers_list') as HTMLSelectElement;
fetch('/api/getProvidersList', {method: 'POST'})
  .then(res => res.json())
  .then((providers: {list: string[], def: string}) => {
      if(select){
          providers.list.forEach(provider => {
              const option = document.createElement('option');
              option.setAttribute('value', provider);
              if(provider == providers.def){
                  option.setAttribute('selected', 'selected');
              }
              option.innerHTML = provider;
              select.appendChild(option);
          });
      }
  });
// повесим обработчик на выбор провайдера
select.addEventListener('change', (evt) => {
    const provider = select.options[select.selectedIndex].value;
    current_provider = provider;
    table.trigger('reloadGrid');
});
}
