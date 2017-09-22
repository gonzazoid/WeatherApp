/// <reference path="./tsd.d.ts" />
import { ajaxRowOptions, ajaxGridOptions } from './jqGridOptions';

{
const table = $('#locations_list');
console.log('new front end!!!');

const deleteLink = function(cellValue: any, options: any, rowdata: any, action: any)  {
    return '<a href="/" data-id="' + (rowdata.placeId ? rowdata.placeId : '') + '" class="ui-icon ui-icon-closethick remover"></a>';
};
(deleteLink as any).reformatAfterEdit = true;

(table.jqGrid as any)({
    url:'/api/getLocations',
    editurl: '/api/setLocation',
    datatype: 'json',
    serializeGridData: JSON.stringify,
    ajaxGridOptions,
    serializeRowData: function(postData: any){
        if(postData.placeId && isNaN(parseInt(postData.placeId, 10))){
            postData.placeId = 0;
        }
        return JSON.stringify(postData);
    },
    ajaxRowOptions,
    colNames:['#', 'City', 'by openWeather', 'by Yahoo', '[ ]'],
    colModel:[
        {name:'placeId', index:'placeId', width:55, sortable: false, key: true},
        {name:'title', index:'title', width:160, editable: true, formatter: 'showlink', formatoptions: {baseLinkUrl: 'Home/viewHistory', idName: 'placeId'}},
        {name:'openWeatherProvidersAlias', width:160, sortable: false, editable: true},
        {name:'yahooProvidersAlias', width:160, sortable: false, editable: true},
        {name:'delete', search:false, index:'placeId', width:45, sortable: false, formatter: deleteLink}
    ],
    rowNum:10,
    rowList:[10,20,30],
    pager: '#pager',
    sortname: 'placeId',
    viewrecords: true,
    sortorder: 'desc',
    caption:'Locations List'
});


(table.jqGrid as any)('navGrid','#pager',{edit:false,add:false,del:false},
    {
        serializeGridData: JSON.stringify,
        ajaxGridOptions
    },
    {
        serializeGridData: JSON.stringify,
        ajaxGridOptions
    });

table.jqGrid('inlineNav','#pager', {
    cancel: true,
    addParams: { 
        addRowParams: {
            keys: true,
            aftersavefunc: function (rowid: string, response: JQuery.jqXHR, options: any) {
                $('#' + rowid).attr('id', response.responseJSON.placeId);
                const data = response.responseJSON;
                data.delete = '';
                table.jqGrid('setRowData', response.responseJSON.placeId, data);
                updateActions();
            }
        }
    }
});

const updateActions = function(e?: any, rowid?: any, loadEvent?: any) {
    console.log('links!!!', document.querySelectorAll('.remover'));
    [].forEach.call(document.querySelectorAll('.remover'), (a: HTMLElement)=>{
        const rowid = a.getAttribute('data-id');
        a.addEventListener('click', function(evt: Event){
            console.log(this);
            const row = table.jqGrid('getRowData', rowid);
            delete row.delete;
            row.oper = 'delete';
            console.log(row);
            evt.preventDefault();
            const headers = new Headers();
            headers.append('Content-Type', 'application/json');
            fetch('/api/setLocation', {
                method: 'POST'
               ,headers
               ,body: JSON.stringify(row)
            }).then(()=>{
                table.jqGrid('delRowData', rowid);
            });
        });
        console.log(a);
    });
};

table.bind('jqGridLoadComplete', updateActions);

}
