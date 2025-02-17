  Declare @asvID int
  DECLARE @cstId int
  set @cstId=(select CST_ID FROM TS_CONFIG_SETTING_TYPE WHERE CST_CODE='APP')
  IF(@cstId is null)
  BEGIN
	INSERT INTO TS_CONFIG_SETTING_TYPE(CST_CODE,CST_VALUE)
	VALUES('APP','APP CONFIG')
	set @cstId=(select CST_ID FROM TS_CONFIG_SETTING_TYPE WHERE CST_CODE='APP')
  END

  set @asvID=(select max(asv_id) from TM_APP_SETTING_CONFIG_VERSION where ASV_CST_ID=@cstId)

  IF @asvID IS NULL
  BEGIN
	INSERT INTO TM_APP_SETTING_CONFIG_VERSION (ASV_VERSION,ASV_CST_ID)
	VALUES(1.00,@cstId)
	set @asvID=(select max(asv_id) from TM_APP_SETTING_CONFIG_VERSION where ASV_CST_ID=@cstId) 
  END


  IF(SELECT COUNT(*) FROM TS_APP_SETTING WHERE APP_ASV_ID=@asvID)>0
  BEGIN
	SET @asvID=@asvID + 1
	INSERT INTO TM_APP_SETTING_CONFIG_VERSION(ASV_VERSION,ASV_CST_ID)
	VALUES(@asvID,@cstId)
  END
insert into ts_app_setting (app_asv_id,app_key,app_value,APP_DESCRIPTION,APP_SENSITIVE_VALUE)
values (@asvId,'config.client.json','{
	"organisation": "",
	"corsDomain": "",
	"url": {
		"api": {
			"jsonrpc": {
				"public": "",
				"private": ""
			}
		}
	},
	"template": {
		"footer": {
			"contact": {
				"address": "Your full address here.",
				"phone": "",
				"email": ""
			},
			"social": [
				{
                    "name": "github",
                    "icon": "fab fa-github"
                }
			],
			"links": [
				{
					"text": "",
					"url": ""
				}
			],
			"watermark": {
				"src": "",
				"alt": "",
				"url": ""
			}
		}
	},
	"mask": {
		"datetime": {
			"ajax": "YYYY-MM-DDTHH:mm:ss",
			"display": "DD/MM/YYYY HH:mm:ss",
			"file": "YYYYMMDDTHHMMss",
			"dateRangePicker": "DD/MM/YYYY HH:mm"
		},
		"date": {
			"ajax": "YYYY-MM-DD",
			"display": "DD/MM/YYYY",
			"dateRangePicker": "DD/MM/YYYY"
		},
		"time": {
			"display": "HH:mm:ss"
		}
	},
	"transfer": {
		"timeout": 3600000,
		"threshold": {
			"soft": 1048576,
			"hard": 1073741824
		},
		"unitsPerSecond": {
			"PxStat.Build.Build_API.Validate": 250000,
			"PxStat.Build.Build_API.Read": 250000,
			"PxStat.Build.Build_API.ReadTemplate": 250000,
			"PxStat.Build.Build_API.ReadDataset": 250000,
			"PxStat.Build.Build_API.Update": 40000,
			"PxStat.Data.Matrix_API.Validate": 500000,
			"PxStat.Data.Matrix_API.Create": 700000,
			"PxStat.Data.GeoMap_API.Validate": 500000
		}
	},
	"entity": {
		"data": {
			"datatable": {
				"length": 100,
				"null": ".."
			},
			"threshold": {
				"soft": 1000,
				"hard": 1000000
			},
			"pagination": 10,
			"lastUpdatedTables": {
				"defaultPageLength": 10,
				"defaultNumDaysFrom": 6
			},
			"chartJs": {
				"chart": {
					"enabled": true
				},
				"map": {
					"enabled": true
				}
			},
			"snippet": null,
			"properties": {
				"archive": "",
				"underReservation": "",
				"experimental": ""
			}
		},
		"map": {
			"baseMap": {
				"leaflet": [
					{
						"url": "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
						"options": {
							"attribution": "&copy; <a target=\"_blank\" href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a>"
						}
					}
				],
				"esri": []
			}
		},
		"build": {
			"threshold": {
				"soft": 1000000,
				"geoJson": 3145728
			},
			"geoJsonLookup": {
				"enabled": true,
				"href": ""
			}
		},
		"openAccess": {
			"recaptcha": {
				"siteKey": "6"
			},
			"authenticator": ""
		},
		"release": {
			"comparison": {
				"threshold": {
					"soft": 1048576
				},
				"differenceClass": "table-danger"
			}
		},
		"analytic": {
			"dateRangePicker": 7
		}
	},
	"plugin": {
		"sharethis": {
			"enabled": true,
			"apiURL": "https://platform-api.sharethis.com/js/sharethis.js#property={0}&product=inline-share-buttons",
			"apiKey": ""
		},
		"jscookie": {
			"session": {
				"path": "/",
				"secure": "true"
			},
			"persistent": {
				"path": "/",
				"secure": "true",
				"expires": 365
			}
		},
		"datatable": {
			"lengthMenu": [
				[
					10,
					25,
					50,
					100,
					-1
				],
				[
					10,
					25,
					50,
					100,
					"All"
				]
			],
			"responsive": true,
			"fixedHeader": true,
			"dom": "fltip",
			"deferRender": true
		},
		"chartJs": {
			"chart": {
				"options": {
					"responsive": true,
					"maintainAspectRatio": false,
					"title": {
						"display": true,
						"text": []
					},
					"tooltips": {
						"mode": "index",
						"intersect": false,
						"callbacks": {
							"label": null
						}
					},
					"hover": {
						"mode": "nearest",
						"intersect": true
					},
					"scales": {
						"xAxes": [
							{
								"ticks": {
									"beginAtZero": false,
									"maxTicksLimit": null
								},
								"gridLines": {
									"display": false
								},
								"scaleLabel": {
									"display": false,
									"labelString": null
								}
							}
						],
						"yAxes": [
							{
								"display": true,
								"position": "left",
								"id": null,
								"ticks": {
									"beginAtZero": false
								},
								"callback": null,
								"scaleLabel": {
									"display": false,
									"labelString": null
								}
							}
						]
					},
					"plugins": {
						"stacked100": {
							"enable": false,
							"replaceTooltipLabel": false
						},
						"colorschemes": {
							"scheme": null
						}
					},
					"legend": {
						"display": true,
						"position": "bottom"
					},
					"elements": {
						"line": {
							"tension": 0.4
						}
					}
				},
				"colours": [
					"#405381",
					"#5BC1A5",
					"#FCBE72",
					"#0091AB",
					"#90989F",
					"#3D5999",
					"#C6CC5C",
					"#579599",
					"#FBAA34",
					"#007780",
					"#B9BEC3",
					"#00AF86",
					"#6BC2C2",
					"#00758C",
					"#FCB053",
					"#6A7794",
					"#A3CCC1",
					"#F68B58",
					"#36A0B3"
				]
			}
		},
		"subscriber": {
			"enabled": true,
			"firebase": {
				"config": {
					"apiKey": "",
					"authDomain": "",
					"projectId": "",
					"storageBucket": "",
					"messagingSenderId": "",
					"appId": ""
				},
				"providers": {
					"emailPassword": true,
					"google": true,
					"facebook": true,
					"gitHub": true,
					"twitter": true
				}
			}
		},
		"leaflet": {
			"colourScale": [
				{
					"value": "red",
					"name": "red"
				},
				{
					"value": "yellow",
					"name": "yellow"
				},
				{
					"value": "blue",
					"name": "blue"
				},
				{
					"value": "darkorange",
					"name": "orange"
				},
				{
					"value": "darkviolet",
					"name": "violet"
				}
			],
			"mode": [
				{
					"value": "q",
					"label": "quantile"
				},
				{
					"value": "e",
					"label": "equidistant"
				},
				{
					"value": "k",
					"label": "k-means"
				}
			],
			"defaultMode": "q",
			"baseMap": {
				"leaflet": [],
				"esri": []
			}
		}
	}
}','Json config for config.client.json','0' )
insert into ts_app_setting (app_asv_id,app_key,app_value,APP_DESCRIPTION,APP_SENSITIVE_VALUE)
values (@asvId,'config.global.json','{
    "title": "",
    "language": {
        "iso": {
            "code": "en",
            "name": "English"
        },
        "culture": "en-ie"
    },
    "url": {
        "api": {
            "restful": "",
            "static": ""
        },
        "logo": "",
        "application": ""
    },
    "dataset": {
        "officialStatistics": true,
        "download": {
            "threshold": {
                "csv": 1048575,
                "xlsx": 1048575
            }
        },
        "analytical": {
            "label": "dependency",
            "icon": "fa-solid fa-circle-nodes",
            "colour": "text-analytical",
            "display": true
        }
    },
    "regex": {
        "phone": {
            "pattern": "^(.*)$",
            "placeholder": "(+111)111111111"
        },
        "password": "^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[\\W]).{8,}$",
        "matrix-name": ".*",
        "product-code": "^[a-zA-Z0-9]+$"
    },
    "workflow": {
        "embargo": {
            "time": "08:00:00",
            "day": [
                1,
                2,
                3,
                4,
                5
            ]
        },
        "fastrack": {
            "response": {
                "approver": true,
                "poweruser": true,
                "administrator": true
            },
            "signoff": {
                "poweruser": true,
                "administrator": true
            }
        },
        "release": {
            "reasonRequired": false
        }
    },
    "build": {
        "create": {
            "moderator": true
        },
        "update": {
            "moderator": true
        },
        "import": {
            "moderator": true
        }
    },
    "session": {
        "length": 1200
    },
    "security": {
        "adOpenAccess": false,
        "demo": true,
        "tokenApiAccessIpMaskWhitelist": [
            {
                "prefix": "255.255.255.255",
                "length": 0
            }
        ]
    },
    "search": {
        "maximumResults": 100
    },
    "report": {
        "date-validation": {
            "minDate": 365,
            "maxDate": -1
        }
    }
}','Json config for config.global.json','0' )
insert into ts_app_setting (app_asv_id,app_key,app_value,APP_DESCRIPTION,APP_SENSITIVE_VALUE)
values (@asvId,'config.server.json','{
    "bulkcopy-tranche-multiplier": 10,
    "px": {
        "confidential-value": "..",
        "default-units": "-"
    },
    "analytic": {
        "read-os-item-limit": 10,
        "read-browser-item-limit": 10,
        "read-referrer-item-limit": 100,
        "referrer-not-applicable": "n/a",
        "read-environment-language-limit": 100
    },
    "search": {
        "synonym-multiplier": 1,
        "search-word-multiplier": 15,
        "release_word_multiplier": 1,
        "product_word_multiplier": 10,
        "subject_word_multiplier": 100
    },
    "language": {
        "iso": {
            "code": "en",
            "name": "English"
        }
    },
    "release": {
        "lockTimeMinutes": 60,
        "defaultReason": "02ROUTINE"
    },
    "subscription": {
        "query-threshold": 1000
    },
    "maintenance": {
        "post-import-delete": false
    },
    "throttle": {
        "subscribedWindowSeconds": 60,
        "subscribedCallLimit": 30,
        "nonSubscribedWindowSeconds": 180,
        "nonSubscribedCallLimit": 10
    },
    "whitelist": [
        "cso.ie"
    ],
    "pxapiv1": {
        "formats": [
            "json-stat",
            "json-stat2",
            "csv",
            "px",
            "xlsx"
        ]
    }
}','Json config for config.server.json','0' )
